using System;

namespace Attributes
{
    /// <summary>
    /// annotation for after testing 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterClassAttribute : Attribute
    {
    }
}
