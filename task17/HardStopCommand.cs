using System;
using System.Threading;

namespace task17
{
    public class HardStopCommand : ICommand
    {
        private ServerThread TargetServer;

        public HardStopCommand(ServerThread TargetServer)
        {
            this.TargetServer = TargetServer;
        }

        public void Execute()
        {
            if (Thread.CurrentThread != TargetServer.GetThread())
            {
                throw new InvalidOperationException("HardStop must be executed in the target thread");
            }

            TargetServer.RequestHardStop();
        }
    }
}
