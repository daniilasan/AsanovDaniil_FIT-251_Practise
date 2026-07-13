using System;
using System.Threading;

namespace task17
{
    public class SoftStopCommand : ICommand
    {
        private ServerThread TargetServer;

        public SoftStopCommand(ServerThread TargetServer)
        {
            this.TargetServer = TargetServer;
        }

        public void Execute()
        {
            if (Thread.CurrentThread != TargetServer.GetThread())
            {
                throw new InvalidOperationException("SoftStop must be executed in the target thread");
            }

            TargetServer.RequestSoftStop();
        }
    }
}
