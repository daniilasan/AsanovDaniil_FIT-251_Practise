using Xunit;
using task17;
using System;
using System.Threading;

namespace task17tests
{
    public class ServerThreadTests
    {
        private class DummyCommand : ICommand
        {
            public bool IsExecuted = false;
            public int SleepTime = 0;

            public DummyCommand(int SleepTime)
            {
                this.SleepTime = SleepTime;
            }

            public void Execute()
            {
                if (SleepTime > 0)
                {
                    Thread.Sleep(SleepTime);
                }
                IsExecuted = true;
            }
        }

        private class FailingCommand : ICommand
        {
            public void Execute()
            {
                throw new Exception("Test exception");
            }
        }

        [Fact]
        public void HardStop_StopsThread_Immediately()
        {
            ServerThread Server = new ServerThread(null);
            DummyCommand Cmd1 = new DummyCommand(50);
            DummyCommand Cmd2 = new DummyCommand(10);
            DummyCommand Cmd3 = new DummyCommand(10);

            Server.Enqueue(Cmd1);
            Server.Enqueue(new HardStopCommand(Server));
            Server.Enqueue(Cmd2);
            Server.Enqueue(Cmd3);
            Thread.Sleep(200);

            Assert.True(Cmd1.IsExecuted);
            Assert.False(Cmd2.IsExecuted);
            Assert.False(Cmd3.IsExecuted);
            Assert.False(Server.GetThread().IsAlive);
        }

        [Fact]
        public void SoftStop_WaitsForQueueToEmpty()
        {
            ServerThread Server = new ServerThread(null);
            DummyCommand Cmd1 = new DummyCommand(50);
            DummyCommand Cmd2 = new DummyCommand(50);
            DummyCommand Cmd3 = new DummyCommand(50);

            Server.Enqueue(Cmd1);
            Server.Enqueue(Cmd2);
            Server.Enqueue(Cmd3);
            Server.Enqueue(new SoftStopCommand(Server));

            Thread.Sleep(300);

            Assert.True(Cmd1.IsExecuted);
            Assert.True(Cmd2.IsExecuted);
            Assert.True(Cmd3.IsExecuted);
            Assert.False(Server.GetThread().IsAlive);
        }

        [Fact]
        public void HardStop_FromWrongThread_ThrowsException()
        {
            ServerThread Server = new ServerThread(null);
            HardStopCommand Cmd = new HardStopCommand(Server);

            Assert.Throws<InvalidOperationException>(() => Cmd.Execute());
        }

        [Fact]
        public void SoftStop_FromWrongThread_ThrowsException()
        {
            ServerThread Server = new ServerThread(null);
            SoftStopCommand Cmd = new SoftStopCommand(Server);

            Assert.Throws<InvalidOperationException>(() => Cmd.Execute());
        }

        [Fact]
        public void ExceptionHandler_CatchesExceptions()
        {
            ICommand? FailedCommand = null;
            Exception? CaughtException = null;

            ExceptionHandler Handler = (ICommand Cmd, Exception Ex) =>
            {
                FailedCommand = Cmd;
                CaughtException = Ex;
            };

            ServerThread Server = new ServerThread(Handler);
            FailingCommand Cmd = new FailingCommand();

            Server.Enqueue(Cmd);
            Server.Enqueue(new SoftStopCommand(Server));

            Thread.Sleep(100);

            Assert.Equal(Cmd, FailedCommand);
            Assert.NotNull(CaughtException);
            Assert.Equal("Test exception", CaughtException.Message);
        }
    }
}
