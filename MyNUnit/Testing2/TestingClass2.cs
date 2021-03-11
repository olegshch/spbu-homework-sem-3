using Attributes;

namespace Testing2
{

    public class TestingClass2
    {
        public static bool ignored;

        [Test(Ignore = "ignore")]
        public void IgnoredTest()
        {
            ignored = false;
        }
    }
}
