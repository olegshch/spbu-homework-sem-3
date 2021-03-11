using System;

namespace Attributes
{
    /// <summary>
    /// annotation for after test 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterAttribute : Attribute
    {
    }
}
