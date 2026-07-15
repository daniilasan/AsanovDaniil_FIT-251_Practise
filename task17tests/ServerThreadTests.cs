using Xunit;
using task17;
using System;
using System.Threading;

namespace task17tests
{
    public class ServerThreadTests
    {
        private class LongCommand : ICommand
        {
            public int TicksRemaining;
            public int ExecutedCount = 0;

            public LongCommand(int Ticks)
            {
                TicksRemaining = Ticks;
            }

            public bool Execute()
            {
                ExecutedCount++;
                TicksRemaining--;
                Thread.Sleep(5);
                return TicksRemaining == 0;
            }
        }

        [Fact]
        public void Scheduler_ExecutesLongCommand_MultipleTimes()
        {
            RoundRobinScheduler Scheduler = new RoundRobinScheduler();
            ServerThread Server = new ServerThread(null, Scheduler);
            LongCommand Cmd = new LongCommand(3);

            Server.Enqueue(Cmd);
            Thread.Sleep(200);

            Assert.Equal(3, Cmd.ExecutedCount);

            Server.RequestHardStop();
            Server.GetThread().Join();
        }

        [Fact]
        public void HardStop_StopsThread_Immediately()
        {
            RoundRobinScheduler Scheduler = new RoundRobinScheduler();
            ServerThread Server = new ServerThread(null, Scheduler);

            LongCommand Cmd1 = new LongCommand(10);
            LongCommand Cmd2 = new LongCommand(10);

            Server.Enqueue(Cmd1);
            Server.Enqueue(new HardStopCommand(Server));
            Server.Enqueue(Cmd2);

            Thread.Sleep(200);

            Assert.True(Cmd1.ExecutedCount > 0);
            Assert.True(Cmd1.ExecutedCount < 10);
            Assert.Equal(0, Cmd2.ExecutedCount);
            Assert.False(Server.GetThread().IsAlive);
        }

        [Fact]
        public void HardStop_FromWrongThread_ThrowsException()
        {
            RoundRobinScheduler Scheduler = new RoundRobinScheduler();
            ServerThread Server = new ServerThread(null, Scheduler);
            HardStopCommand Cmd = new HardStopCommand(Server);

            Assert.Throws<InvalidOperationException>(() => Cmd.Execute());

            Server.RequestHardStop();
            Server.GetThread().Join();
        }
    }
}
