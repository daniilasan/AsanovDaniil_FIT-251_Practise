using System;

namespace CommandLib
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisplayNameAttribute : Attribute
    {
        public string DisplayName { get; set; }

        public DisplayNameAttribute(string DisplayName)
        {
            this.DisplayName = DisplayName;
        }
    }
}
