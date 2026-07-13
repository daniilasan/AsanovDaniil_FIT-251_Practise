using Xunit;
using task17;
using System;
using System.Threading;

namespace task17tests
{
    public class ServerThreadTests
    {
        //Команда, которая выполняется за несколько тиков
        private class LongRunningCommand : ICommand
        {
            public int TicksRemaining;
            public int ExecutedCount = 0;

            public LongRunningCommand(int Ticks)
            {
                TicksRemaining = Ticks;
            }

            public bool Execute()
            {
                Thread.Sleep(10);

                ExecutedCount++;
                TicksRemaining--;
                return TicksRemaining == 0;
            }
        }

        //Команда, которая помечает себя выполненной сразу
        private class InstantCommand : ICommand
        {
            public bool WasExecuted = false;

            public bool Execute()
            {
                WasExecuted = true;
                return true;
            }
        }

        [Fact]
        public void LongRunningCommand_ExecutesMultipleTimes()
        {
            RoundRobinScheduler Scheduler = new RoundRobinScheduler();
            ServerThread Server = new ServerThread(null, Scheduler, 10);
            LongRunningCommand Cmd = new LongRunningCommand(5);

            Server.Enqueue(Cmd);

            //Ждём, пока команда отработает все 5 тиков
            Thread.Sleep(200);

            Assert.Equal(5, Cmd.ExecutedCount);

            Server.Stop();
            Thread.Sleep(50);
        }

        [Fact]
        public void RoundRobin_AlternatesBetweenCommands()
        {
            RoundRobinScheduler Scheduler = new RoundRobinScheduler();
            ServerThread Server = new ServerThread(null, Scheduler, 10);

            LongRunningCommand Cmd1 = new LongRunningCommand(3);
            LongRunningCommand Cmd2 = new LongRunningCommand(3);

            Server.Enqueue(Cmd1);
            Server.Enqueue(Cmd2);

            // Ждем пока обе отработают
            Thread.Sleep(200);

            Assert.Equal(3, Cmd1.ExecutedCount);
            Assert.Equal(3, Cmd2.ExecutedCount);

            Server.Stop();
            Thread.Sleep(50);
        }

        [Fact]
        public void InstantCommand_DoesNotBlock_LongRunning()
        {
            RoundRobinScheduler Scheduler = new RoundRobinScheduler();
            ServerThread Server = new ServerThread(null, Scheduler, 10);

            LongRunningCommand LongCmd = new LongRunningCommand(10);
            Server.Enqueue(LongCmd);
            Thread.Sleep(50);
            InstantCommand InstantCmd = new InstantCommand();
            Server.Enqueue(InstantCmd);

            Thread.Sleep(50);

            Assert.True(InstantCmd.WasExecuted);
            Assert.True(LongCmd.ExecutedCount < 10); 

            Server.Stop();
            Thread.Sleep(50);
        }

        [Fact]
        public void ServerThread_Stops_WhenEmpty()
        {
            RoundRobinScheduler Scheduler = new RoundRobinScheduler();
            ServerThread Server = new ServerThread(null, Scheduler, 10);
            Server.Stop();
            Thread.Sleep(100);

            Assert.False(Server.GetThread().IsAlive);
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

            RoundRobinScheduler Scheduler = new RoundRobinScheduler();
            ServerThread Server = new ServerThread(Handler, Scheduler, 10);
            var FailingCmd = new FailingCommand(); Server.Enqueue(FailingCmd);

            Thread.Sleep(100);

            Assert.Equal(FailingCmd, FailedCommand);
            Assert.NotNull(CaughtException);
            Assert.Equal("Test exception", CaughtException.Message);

            Server.Stop();
            Thread.Sleep(50);
        }

        private class FailingCommand : ICommand
        {
            public bool Execute()
            {
                throw new Exception("Test exception");
            }
        }
    }
}
