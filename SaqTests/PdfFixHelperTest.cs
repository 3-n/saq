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

        [Test]
        public void ShouldNotIntroduceCarriageReturns()
        {
            const string s = @"(jestem )Tj<012e0105>Tj( i omegom.)Tj";
            var s2 = PdfFixHelper.GetFixedString(s);
            Assert.IsFalse(s2.Contains('\r'));
        }

        [Test]
        public void ShouldFixCapitalizedEntity()
        {
            const string s = @"(zale)Tj/C2_1 1 Tf0 Tc 0 Tw 6.627 0 Td<012A>Tj/TT3 1 Tf0.0011 Tc -0.0014 Tw 0.498 0 Td(y)Tj";
            var s2 = PdfFixHelper.GetFixedString(s);
            Assert.IsTrue(s2.Contains('ż'));
        }

        [Test]
        public void ShouldFixNumberedEntity()
        {
            const string s = @"m\363zg";
            var s2 = PdfFixHelper.GetFixedString(s);
            Assert.IsTrue(s2.Contains('ó'));
        }


    }
}
