using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vsix.Common.Helpers;

namespace Vsix.CommonTests.Helpers
{
    [TestFixture()]
    public class DebugHelperTests
    {
        [Test()]
        public void DebugOpenTextTest()
        {
            var list = new List<string> {"hello ", Environment.UserName, DateTime.Now.ToString("g")};
            var tmp = DebugHelper.OpenText(list, ".txt", null);
            Assert.IsNotEmpty(tmp);
        }


        [Test()]
        public void DebugOpenHtmlTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void DebugOpenHtmlTest1()
        {
            Assert.Fail();
        }

        [Test()]
        public void OpenTextAsFileTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void OpenTextTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ToHtmlTableTest()
        {
            Assert.Fail();
        }
    }
}