using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing1;
using Testing4;
using Testing5;

namespace MyNUnit.Tests
{
    [TestClass()]
    public class RunnerTests
    {
        [TestMethod()]
        public void RunTest()
        {
            TestingClass1.count = 4;
            MyNUnit.Runner.Run("..\\..\\..\\Testing1");
            Assert.AreEqual(5, TestingClass1.count);
        }

        [TestMethod()]
        public void IgnoreAnnotationsMessageTest()
        {
            MyNUnit.Runner.Run("..\\..\\..\\Testing2");
            Assert.AreEqual("ignore", MyNUnit.Runner.TestInformation.Take().Ignore);
        }

        [TestMethod()]
        public void FailedDueToExceptionTest()
        {
            MyNUnit.Runner.Run("..\\..\\..\\Testing3");
            Assert.IsFalse(MyNUnit.Runner.TestInformation.Take().IsPassed);
        }

        [TestMethod()]
        public void SimpleWithAttributesBeforeAndAfterTest()
        {
            TestingClass4.array = new bool[]{ false, false, false, false };
            MyNUnit.Runner.Run("..\\..\\..\\Testing4");
            Assert.IsTrue(TestingClass4.array[0]);
            Assert.IsTrue(TestingClass4.array[1]);
            Assert.IsFalse(TestingClass4.array[2]);
            Assert.IsTrue(TestingClass4.array[3]);
        }

        [TestMethod()]
        public void MenhodWithAttributesBeforeClassAndAfterClassTest()
        {
            TestingClass5.array = new bool[] { false, false };
            MyNUnit.Runner.Run("..\\..\\..\\Testing5");
            Assert.AreEqual(true, TestingClass5.array[0]);
            Assert.AreEqual(true, TestingClass5.array[1]);
        }
    }
}