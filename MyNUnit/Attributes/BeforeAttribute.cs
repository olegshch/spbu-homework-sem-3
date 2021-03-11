using System;

namespace Attributes
{
    /// <summary>
    /// annotation for before test 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeAttribute : Attribute
    {
    }
}
