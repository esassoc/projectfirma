/*-----------------------------------------------------------------------
<copyright file="ErrorEmailThrottle.cs" company="Environmental Science Associates">
Copyright (c) Environmental Science Associates. All rights reserved.
<author>Environmental Science Associates</author>
</copyright>

<license>
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License <http://www.gnu.org/licenses/> for more details.

Source code is available upon request via <support@sitkatech.com>.
</license>
-----------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LtInfo.Common
{
    /// <summary>
    /// Keeps App Alerts email from flooding the inbox:
    /// * The first error with a given signature (path + exception + message) is always emailed right away, so new problems and outages still alert immediately.
    /// * Repeats of that signature within <see cref="Window"/> are counted, not emailed. The next email for that signature reports how many were suppressed.
    /// * If more than <see cref="MaxEmailsPerMinute"/> emails would go out in a minute (e.g. database down, every page failing), one "alert storm" email is sent
    ///   and everything else is held for <see cref="StormPause"/>. The next email after that reports how many were suppressed.
    /// * Errors whose window has expired with repeats that were never reported are summarized on the next email that goes out.
    /// FATAL events are always sent: they skip the per-signature check and the storm pause (and never start a storm themselves).
    /// </summary>
    public class ErrorEmailThrottle
    {
        public class Decision
        {
            public bool ShouldSend { get; set; }
            public string SubjectTag { get; set; }
            public string BodyPreamble { get; set; }
        }

        private class SignatureWindow
        {
            public DateTime WindowStart;
            public int SuppressedCount;
        }

        private const int MaxTrackedSignatures = 1000;
        private const int MaxExpiredRepeatSummaryLines = 10;
        private const int MaxSignatureLengthInSummary = 150;
        private static readonly TimeSpan DefaultStormPause = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        private static readonly Decision DoNotSend = new Decision { ShouldSend = false };

        private static readonly Regex GuidRegex = new Regex(@"[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex QuotedTextRegex = new Regex(@"'[^']*'|""[^""]*""", RegexOptions.Compiled);
        private static readonly Regex DigitsRegex = new Regex(@"\d+", RegexOptions.Compiled);
        private static readonly Regex WhitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);

        public TimeSpan Window { get; }
        public int MaxEmailsPerMinute { get; }

        /// <summary>
        /// Storms reuse the repeat window, but still pause when the repeat window is turned off so the two settings are independent
        /// </summary>
        public TimeSpan StormPause => Window > TimeSpan.Zero ? Window : DefaultStormPause;

        private readonly object _lock = new object();
        private readonly Dictionary<string, SignatureWindow> _signatureWindows = new Dictionary<string, SignatureWindow>();
        private readonly Queue<DateTime> _recentSends = new Queue<DateTime>();
        private DateTime? _stormStart;
        private int _stormSuppressedCount;

        /// <param name="window">How long repeats of the same error (and an alert storm) are held. Zero or less turns off per-signature throttling, storms then pause for 15 minutes.</param>
        /// <param name="maxEmailsPerMinute">Emails per minute before an alert storm is declared. Zero or less turns off the storm cap.</param>
        public ErrorEmailThrottle(TimeSpan window, int maxEmailsPerMinute)
        {
            Window = window;
            MaxEmailsPerMinute = maxEmailsPerMinute;
        }

        public Decision Evaluate(string signature, bool isFatal, DateTime now)
        {
            lock (_lock)
            {
                var notes = new List<string>();
                var subjectTags = new List<string>();

                SignatureWindow signatureWindow;
                _signatureWindows.TryGetValue(signature, out signatureWindow);
                var isRepeatWithinWindow = signatureWindow != null && Window > TimeSpan.Zero && now - signatureWindow.WindowStart < Window;
                if (isRepeatWithinWindow && !isFatal)
                {
                    signatureWindow.SuppressedCount++;
                    return DoNotSend;
                }

                if (_stormStart.HasValue)
                {
                    if (now - _stormStart.Value < StormPause)
                    {
                        if (!isFatal)
                        {
                            _stormSuppressedCount++;
                            return DoNotSend;
                        }
                        // FATAL always goes out, the storm carries on around it
                        notes.Add($"Sent during an alert storm (started {_stormStart.Value:HH:mm}) because it is FATAL.");
                    }
                    else
                    {
                        notes.Add($"Alert storm over: {_stormSuppressedCount} error email(s) were suppressed between {_stormStart.Value:HH:mm} and {now:HH:mm}. Check the log file for details.");
                        _stormStart = null;
                        _stormSuppressedCount = 0;
                    }
                }

                while (_recentSends.Count > 0 && now - _recentSends.Peek() >= OneMinute)
                {
                    _recentSends.Dequeue();
                }
                if (MaxEmailsPerMinute > 0 && !_stormStart.HasValue && _recentSends.Count >= MaxEmailsPerMinute)
                {
                    _stormStart = now;
                    subjectTags.Add("[ALERT STORM]");
                    notes.Add($"ALERT STORM: more than {MaxEmailsPerMinute} error emails in the last minute. Error emails are paused for {StormPause.TotalMinutes:0} minutes. Check that the site and its database are up, and check the log file.");
                }

                if (signatureWindow != null && signatureWindow.SuppressedCount > 0)
                {
                    subjectTags.Add($"[+{signatureWindow.SuppressedCount} repeats]");
                    notes.Add($"This error happened {signatureWindow.SuppressedCount} more time(s) since {signatureWindow.WindowStart:HH:mm} that were not emailed.");
                }
                if (Window > TimeSpan.Zero)
                {
                    notes.Add($"Repeats of this error in the next {Window.TotalMinutes:0} minutes will not be emailed.");
                }
                notes.AddRange(TakeExpiredRepeatSummary(signature, now));

                RecordSend(signature, now);
                return new Decision
                {
                    ShouldSend = true,
                    SubjectTag = string.Join(" ", subjectTags),
                    BodyPreamble = string.Join(Environment.NewLine, notes)
                };
            }
        }

        /// <summary>
        /// Summarizes (and stops tracking) other errors whose window has expired with repeats that were never reported because the error didn't come back.
        /// Expired entries with no repeats are dropped too, so tracking only holds errors that are still inside their window.
        /// </summary>
        private List<string> TakeExpiredRepeatSummary(string currentSignature, DateTime now)
        {
            var summary = new List<string>();
            if (Window <= TimeSpan.Zero)
            {
                return summary;
            }

            var expired = _signatureWindows.Where(x => x.Key != currentSignature && now - x.Value.WindowStart >= Window).ToList();
            var withUnreportedRepeats = expired.Where(x => x.Value.SuppressedCount > 0).OrderByDescending(x => x.Value.SuppressedCount).ToList();
            if (withUnreportedRepeats.Any())
            {
                summary.Add(string.Empty);
                summary.Add("Other errors that repeated after their last email (not emailed):");
                summary.AddRange(withUnreportedRepeats.Take(MaxExpiredRepeatSummaryLines)
                    .Select(x => $"  {x.Value.SuppressedCount} more time(s) since {x.Value.WindowStart:HH:mm}: {Truncate(x.Key, MaxSignatureLengthInSummary)}"));
                if (withUnreportedRepeats.Count > MaxExpiredRepeatSummaryLines)
                {
                    summary.Add($"  ...and {withUnreportedRepeats.Count - MaxExpiredRepeatSummaryLines} more");
                }
            }
            expired.ForEach(x => _signatureWindows.Remove(x.Key));
            return summary;
        }

        private void RecordSend(string signature, DateTime now)
        {
            _recentSends.Enqueue(now);
            if (Window <= TimeSpan.Zero)
            {
                return;
            }
            // Hard cap: if still full after expired entries were dropped, evict the oldest
            if (!_signatureWindows.ContainsKey(signature) && _signatureWindows.Count >= MaxTrackedSignatures)
            {
                var oldestSignature = _signatureWindows.OrderBy(x => x.Value.WindowStart).First().Key;
                _signatureWindows.Remove(oldestSignature);
            }
            _signatureWindows[signature] = new SignatureWindow { WindowStart = now, SuppressedCount = 0 };
        }

        /// <summary>
        /// Identifies "the same error": same request path, same exception types and same message, ignoring ids, numbers and guids
        /// </summary>
        public static string BuildSignature(string path, Exception exception, string renderedMessage)
        {
            var normalizedPath = Normalize(path ?? string.Empty).ToLowerInvariant();
            if (exception != null)
            {
                var exceptionChain = new List<Exception>();
                for (var currentException = exception; currentException != null; currentException = currentException.InnerException)
                {
                    exceptionChain.Add(currentException);
                }
                var exceptionTypes = string.Join("--->", exceptionChain.Select(x => x.GetType().Name));
                return $"{normalizedPath}|{exceptionTypes}|{Truncate(Normalize(exceptionChain.Last().Message), 100)}";
            }
            // SitkaLogger puts a blank line between the message and the user/session/request details, only the message identifies the error
            var messageOnly = Regex.Split(renderedMessage ?? string.Empty, @"\r?\n\r?\n").First();
            return $"{normalizedPath}|{Truncate(Normalize(messageOnly), 200)}";
        }

        private static string Normalize(string text)
        {
            // Quoted values are usually user/fuzzer input (e.g. Requested value 'Xxxx' was not found), so they don't make a different error
            var withoutQuotedText = QuotedTextRegex.Replace(text, "'#'");
            var withoutGuids = GuidRegex.Replace(withoutQuotedText, "#");
            var withoutDigits = DigitsRegex.Replace(withoutGuids, "#");
            return WhitespaceRegex.Replace(withoutDigits, " ").Trim();
        }

        private static string Truncate(string text, int maxLength)
        {
            return text.Length <= maxLength ? text : text.Substring(0, maxLength);
        }
    }
}
