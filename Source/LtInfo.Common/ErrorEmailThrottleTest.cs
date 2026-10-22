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
            Assert.That(throttle.Evaluate("page5", true, Start.AddMinutes(6)).ShouldSend, Is.False, "FATAL still counts toward the storm cap");

            var afterStorm = throttle.Evaluate("page6", false, Start.AddMinutes(16));
            Assert.That(afterStorm.ShouldSend, Is.True);
            Assert.That(afterStorm.BodyPreamble, Is.StringContaining("Alert storm over: 2 error email(s) were suppressed"));
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
