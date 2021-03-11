using System;

namespace MyNUnit
{
    /// <summary>
    /// storing properties class for test annotation method
    /// </summary>
    public class TestInfo
    {
        /// <summary>
        /// method name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// time in milliseconds
        /// </summary>
        public long Time { get; }

        /// <summary>
        /// expected for exceptions
        /// </summary>
        public Type Expected { get; }

        /// <summary>
        /// argument ignored
        /// </summary>
        public string Ignore { get; }

        /// <summary>
        /// true if passed
        /// </summary>
        public bool IsPassed { get; }

        /// <summary>
        /// assembly full name 
        /// </summary>
        public string Assembly { get; }

        public TestInfo(string name, string assembly, long time, bool isPassed, Type expected = null, string ignore = null)
        {
            Name = name;
            Time = time;
            IsPassed = isPassed;
            Expected = expected;
            Ignore = ignore;
        }
    }
}
