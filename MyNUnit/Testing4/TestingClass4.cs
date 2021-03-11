using Attributes;

namespace Testing4
{
    public class TestingClass4
    {
        public static bool[] array;

        [Before]
        public void Before1()
        {
            array[0] = true;
        }

        [Before]
        public void Before2()
        {
            array[1] = true;
        }

        [Test]
        public void Test1()
        {
            array[2] = false;
        }

        [After]
        public void After()
        {
            array[3] = true;
        }
    }
}
