using System;
using CommandLib;

namespace TestPluginA
{
    [PluginLoad("PluginA")]
    public class PluginA : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("PluginA executed");
        }
    }
}
