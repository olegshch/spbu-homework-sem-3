using System;

namespace Attributes
{
    /// <summary>
    /// annotation for test 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class TestAttribute : Attribute
    {
        public Type Expected { get; set; }
        public string Ignore { get; set; }
    }
}
