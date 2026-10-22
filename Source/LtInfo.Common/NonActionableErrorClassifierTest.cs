/*-----------------------------------------------------------------------
<copyright file="NonActionableErrorClassifierTest.cs" company="Environmental Science Associates">
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
using System.Net;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;

namespace LtInfo.Common
{
    [TestFixture]
    public class NonActionableErrorClassifierTest
    {
        [Test]
        public void ClientCausedExceptionsAreNonActionable()
        {
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(new HttpRequestValidationException("A potentially dangerous Request.Form value was detected")), Is.True);
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(new HttpAntiForgeryException("The anti-forgery token could not be decrypted")), Is.True);
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(new HttpException(403, "Forbidden")), Is.True);
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(new HttpException(404, "Not Found")), Is.True);
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(new SitkaDisplayErrorExceptionWithHttpCode(HttpStatusCode.Gone, "Tenant X is disabled")), Is.True);
        }

        [Test]
        public void ClientCausedExceptionWrappedInUnhandledExceptionIsNonActionable()
        {
            var wrapped = new HttpUnhandledException("Unhandled", new HttpAntiForgeryException("bad token"));
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(wrapped), Is.True);
        }

        [Test]
        public void ServerSideExceptionsAreActionable()
        {
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(null), Is.False);
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(new DataException("database unavailable")), Is.False);
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(new HttpException(500, "Server error")), Is.False);
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(new SitkaDisplayErrorExceptionWithHttpCode(HttpStatusCode.ServiceUnavailable, "Down for maintenance")), Is.False);
            Assert.That(NonActionableErrorClassifier.IsNonActionableException(new HttpUnhandledException("Unhandled", new NullReferenceException())), Is.False);
        }

        [Test]
        public void CrawlersAreDetectedByUserAgent()
        {
            Assert.That(NonActionableErrorClassifier.IsFromCrawler("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)"), Is.True);
            Assert.That(NonActionableErrorClassifier.IsFromCrawler("Mozilla/5.0 (compatible; Baiduspider/2.0)"), Is.True);
            Assert.That(NonActionableErrorClassifier.IsFromCrawler("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0 Safari/537.36"), Is.False);
            Assert.That(NonActionableErrorClassifier.IsFromCrawler(null), Is.False);
        }
    }
}
