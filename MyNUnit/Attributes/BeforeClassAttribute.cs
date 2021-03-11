using System;

namespace Attributes
{
    /// <summary>
    /// annotation for before testing 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeClassAttribute : Attribute
    {
    }
}
