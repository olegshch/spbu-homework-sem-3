using Attributes;

namespace Testing1
{
    public class TestingClass1
    {
        public static int count;

        [Test]
        public void Incrementing()
        {
            ++count;
        }
    }
}
