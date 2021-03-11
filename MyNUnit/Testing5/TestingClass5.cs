using Attributes;

namespace Testing5
{
    public class TestingClass5
    {
        public static bool[] array = { false, false };

        [BeforeClass]
        public static void Before1()
        {
            array[0] = !array[0];
        }

        [Before]
        public void Before2()
        {
            array[0] = !array[0];
        }

        [Test]
        public void Test1()
        {
            array[0] = !array[0];
        }

        [AfterClass]
        public static void After()
        {
            array[1] = !array[1];
        }
    }
}
