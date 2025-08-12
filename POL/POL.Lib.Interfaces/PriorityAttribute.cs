using System;

namespace POL.Lib.Interfaces
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PriorityAttribute : Attribute
    {
        public PriorityAttribute(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; private set; }
    }
}
