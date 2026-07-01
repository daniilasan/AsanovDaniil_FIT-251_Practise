using System;

namespace CommandLib
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginLoadAttribute : Attribute
    {
        public string PluginName { get; set; }
        public string[] Dependencies { get; set; }

        public PluginLoadAttribute(string PluginName, params string[] Dependencies)
        {
            this.PluginName = PluginName;

            if (Dependencies == null)
            {
                this.Dependencies = new string[0];
            }
            else
            {
                this.Dependencies = Dependencies;
            }
        }
    }
}
