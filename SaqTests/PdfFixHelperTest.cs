using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SaqFetch;
using NUnit.Framework;

namespace SaqTests
{
    [TestFixture]
    public class PdfFixHelperTest
    {
        [Test]
        public void ShouldReplaceSomething()
        {
            const string s = @"(jestem )Tj<012e0105>Tj( i omegom.)Tj";
            var s2 = PdfFixHelper.GetFixedString(s);
            Assert.AreNotEqual(s, s2);
        }
    }
}
