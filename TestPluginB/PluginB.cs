using System;
using CommandLib;

namespace TestPluginB
{
    [PluginLoad("PluginB", "PluginA")]
    public class PluginB : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("PluginB executed (depends on PluginA)");
        }
    }
}
