using System;

namespace CommandLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public class VersionAttribute : Attribute
    {
        public int Major { get; set; }
        public int Minor { get; set; }

        public VersionAttribute(int Major, int Minor)
        {
            this.Major = Major;
            this.Minor = Minor;
        }
    }
}
