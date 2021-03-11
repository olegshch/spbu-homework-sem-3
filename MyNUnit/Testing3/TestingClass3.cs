using Attributes;
using System;

namespace Testing3
{
    public class TestingClass3
    {
        [Test]
        public void FailedTest()
        {
            throw new Exception();
        }
    }
}
