/*-----------------------------------------------------------------------
<copyright file="ErrorEmailThrottleTest.cs" company="Environmental Science Associates">
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
using System.Data;
using System.Linq;
using NUnit.Framework;

namespace LtInfo.Common
{
    [TestFixture]
    public class ErrorEmailThrottleTest
    {
        private static readonly DateTime Start = new DateTime(2026, 10, 21, 10, 0, 0);

        [Test]
        public void FirstOccurrenceIsSentAndRepeatsWithinWindowAreSuppressed()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.FromMinutes(15), 0);
            Assert.That(throttle.Evaluate("a", false, Start).ShouldSend, Is.True);
            Assert.That(throttle.Evaluate("a", false, Start.AddMinutes(1)).ShouldSend, Is.False);
            Assert.That(throttle.Evaluate("a", false, Start.AddMinutes(14)).ShouldSend, Is.False);
            Assert.That(throttle.Evaluate("b", false, Start.AddMinutes(14)).ShouldSend, Is.True, "A different error is not held back by another error's window");
        }

        [Test]
        public void FirstEmailAfterWindowReportsSuppressedCount()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.FromMinutes(15), 0);
            throttle.Evaluate("a", false, Start);
            throttle.Evaluate("a", false, Start.AddMinutes(1));
            throttle.Evaluate("a", false, Start.AddMinutes(2));

            var decision = throttle.Evaluate("a", false, Start.AddMinutes(16));
            Assert.That(decision.ShouldSend, Is.True);
            Assert.That(decision.SubjectTag, Is.EqualTo("[+2 repeats]"));
            Assert.That(decision.BodyPreamble, Is.StringContaining("2 more time(s) since 10:00"));

            Assert.That(throttle.Evaluate("a", false, Start.AddMinutes(17)).ShouldSend, Is.False, "A new window starts with the email that was just sent");
        }

        [Test]
        public void FatalBypassesPerSignatureThrottle()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.FromMinutes(15), 0);
            throttle.Evaluate("a", false, Start);
            Assert.That(throttle.Evaluate("a", true, Start.AddMinutes(1)).ShouldSend, Is.True);
        }

        [Test]
        public void StormCapSendsOneStormEmailThenHoldsEverythingForTheWindow()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.FromMinutes(15), 3);
            var sent = Enumerable.Range(0, 3).Select(i => throttle.Evaluate("page" + i, false, Start.AddSeconds(i))).ToList();
            Assert.That(sent.All(x => x.ShouldSend), Is.True);

            var stormDecision = throttle.Evaluate("page3", false, Start.AddSeconds(3));
            Assert.That(stormDecision.ShouldSend, Is.True);
            Assert.That(stormDecision.SubjectTag, Is.StringContaining("[ALERT STORM]"));

            Assert.That(throttle.Evaluate("page4", false, Start.AddMinutes(5)).ShouldSend, Is.False);
            Assert.That(throttle.Evaluate("page5", false, Start.AddMinutes(6)).ShouldSend, Is.False);

            var afterStorm = throttle.Evaluate("page6", false, Start.AddMinutes(16));
            Assert.That(afterStorm.ShouldSend, Is.True);
            Assert.That(afterStorm.BodyPreamble, Is.StringContaining("Alert storm over: 2 error email(s) were suppressed"));
        }

        [Test]
        public void FatalDuringStormIsSentWithoutEndingOrRestartingTheStorm()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.FromMinutes(15), 3);
            Enumerable.Range(0, 4).ToList().ForEach(i => throttle.Evaluate("page" + i, false, Start.AddSeconds(i))); // 4th starts the storm

            var fatal = throttle.Evaluate("fatal", true, Start.AddSeconds(10));
            Assert.That(fatal.ShouldSend, Is.True);
            Assert.That(fatal.SubjectTag, Is.Not.StringContaining("[ALERT STORM]"), "A FATAL during a storm doesn't start a new storm");
            Assert.That(fatal.BodyPreamble, Is.StringContaining("because it is FATAL"));

            Assert.That(throttle.Evaluate("page9", false, Start.AddMinutes(1)).ShouldSend, Is.False, "The storm is still on after the FATAL");
            Assert.That(throttle.Evaluate("page10", false, Start.AddMinutes(16)).BodyPreamble, Is.StringContaining("Alert storm over: 1 error email(s)"));
        }

        [Test]
        public void StormStillPausesWhenRepeatWindowIsTurnedOff()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.Zero, 3);
            Assert.That(throttle.StormPause, Is.EqualTo(TimeSpan.FromMinutes(15)));
            Enumerable.Range(0, 3).ToList().ForEach(i => throttle.Evaluate("a", false, Start.AddSeconds(i)));

            var stormDecision = throttle.Evaluate("a", false, Start.AddSeconds(3));
            Assert.That(stormDecision.SubjectTag, Is.EqualTo("[ALERT STORM]"));
            Assert.That(stormDecision.BodyPreamble, Is.StringContaining("paused for 15 minutes"));

            Assert.That(throttle.Evaluate("a", false, Start.AddSeconds(4)).ShouldSend, Is.False);
            Assert.That(throttle.Evaluate("a", false, Start.AddMinutes(5)).ShouldSend, Is.False);
            var afterStorm = throttle.Evaluate("a", false, Start.AddMinutes(16));
            Assert.That(afterStorm.ShouldSend, Is.True);
            Assert.That(afterStorm.SubjectTag, Is.Empty);
        }

        [Test]
        public void RepeatsOfAnErrorThatStoppedAreReportedOnTheNextEmail()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.FromMinutes(15), 0);
            throttle.Evaluate("a", false, Start);
            throttle.Evaluate("a", false, Start.AddMinutes(1));
            throttle.Evaluate("a", false, Start.AddMinutes(2));

            var otherError = throttle.Evaluate("b", false, Start.AddMinutes(16));
            Assert.That(otherError.ShouldSend, Is.True);
            Assert.That(otherError.BodyPreamble, Is.StringContaining("2 more time(s) since 10:00: a"));

            var aAgain = throttle.Evaluate("a", false, Start.AddMinutes(17));
            Assert.That(aAgain.ShouldSend, Is.True, "Once reported, the error starts fresh");
            Assert.That(aAgain.SubjectTag, Is.Empty, "Its repeats were already reported, so not reported twice");
        }

        [Test]
        public void ExpiredRepeatSummaryIsCapped()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.FromMinutes(15), 0);
            for (var i = 0; i < 12; i++)
            {
                throttle.Evaluate("error" + i, false, Start);
                throttle.Evaluate("error" + i, false, Start.AddMinutes(1));
            }

            var decision = throttle.Evaluate("new", false, Start.AddMinutes(16));
            Assert.That(decision.BodyPreamble.Split('\n').Count(x => x.Contains("more time(s) since")), Is.EqualTo(10));
            Assert.That(decision.BodyPreamble, Is.StringContaining("...and 2 more"));
        }

        [Test]
        public void TrackedSignaturesAreHardCapped()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.FromMinutes(15), 0);
            for (var i = 0; i <= 1000; i++)
            {
                throttle.Evaluate("error" + i, false, Start.AddMilliseconds(i));
            }
            Assert.That(throttle.Evaluate("error0", false, Start.AddMinutes(1)).ShouldSend, Is.True, "The oldest signature was evicted to stay at the cap");
            Assert.That(throttle.Evaluate("error1000", false, Start.AddMinutes(1)).ShouldSend, Is.False, "Recent signatures are still tracked");
        }

        [Test]
        public void ZeroSettingsTurnThrottlingOff()
        {
            var throttle = new ErrorEmailThrottle(TimeSpan.Zero, 0);
            Assert.That(Enumerable.Range(0, 20).All(i => throttle.Evaluate("a", false, Start.AddSeconds(i)).ShouldSend), Is.True);
        }

        [Test]
        public void SignatureIgnoresIdsAndNumbers()
        {
            var signature1 = ErrorEmailThrottle.BuildSignature("/Project/Detail/123", new DataException("Timeout after 30000 ms for 0f8fad5b-d9cb-469f-a165-70867728950e"), null);
            var signature2 = ErrorEmailThrottle.BuildSignature("/Project/Detail/456", new DataException("Timeout after 15000 ms for 7c9e6679-7425-40de-944b-e07fc1f90ae7"), null);
            Assert.That(signature1, Is.EqualTo(signature2));

            var differentException = ErrorEmailThrottle.BuildSignature("/Project/Detail/123", new InvalidOperationException("Timeout after 30000 ms"), null);
            Assert.That(differentException, Is.Not.EqualTo(signature1));
        }

        [Test]
        public void SignatureIgnoresQuotedValues()
        {
            var signature1 = ErrorEmailThrottle.BuildSignature("/Results/ProjectMap", new ArgumentException("Requested value 'Xxxx' was not found."), null);
            var signature2 = ErrorEmailThrottle.BuildSignature("/Results/ProjectMap", new ArgumentException("Requested value 'gF3k' was not found."), null);
            Assert.That(signature1, Is.EqualTo(signature2));
        }

        [Test]
        public void SignatureForMessageOnlyIgnoresSessionAndRequestDetails()
        {
            var signature1 = ErrorEmailThrottle.BuildSignature("/Foo", null, "Something broke\r\n\r\nUsername: alice\r\n\r\nIP Address: 1.2.3.4");
            var signature2 = ErrorEmailThrottle.BuildSignature("/Foo", null, "Something broke\r\n\r\nUsername: bob\r\n\r\nIP Address: 5.6.7.8");
            Assert.That(signature1, Is.EqualTo(signature2));
        }
    }
}
