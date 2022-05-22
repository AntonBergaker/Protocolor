using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UnitTests;
class TestTestingUtil {

    [Test]
    public void TestStringToFrame() {
        {
            var str = "hello";
            var frame = TestingUtil.StringToBinaryFrame(str);
            var variable = TestingUtil.BinaryFrameToString(frame);

            Assert.AreEqual(str, variable);
        }
        {
            var str = "hellos";
            var frame = TestingUtil.StringToBinaryFrame(str);
            var variable = TestingUtil.BinaryFrameToString(frame);

            Assert.AreEqual(str, variable);
        }
        {
            var str = "h";
            var frame = TestingUtil.StringToBinaryFrame(str);
            var variable = TestingUtil.BinaryFrameToString(frame);

            Assert.AreEqual(str, variable);
        }
    }
}
