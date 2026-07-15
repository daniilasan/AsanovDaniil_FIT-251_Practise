using System;
using System.Collections.Generic;
using System.Threading;

namespace task17
{
    public class ServerThread
    {
        private Thread WorkerThread;
        private Queue<ICommand> CommandQueue;
        private IScheduler Scheduler;
        private object SyncRoot;
        private bool IsHardStopped;
        private ExceptionHandler? Handler;

        public ServerThread(ExceptionHandler? Handler, IScheduler Scheduler)
        {
            this.Handler = Handler;
            this.Scheduler = Scheduler;
            CommandQueue = new Queue<ICommand>();
            SyncRoot = new object();
            IsHardStopped = false;

            WorkerThread = new Thread(WorkerLoop);
            WorkerThread.IsBackground = true;
            WorkerThread.Start();
        }

        public Thread GetThread()
        {
            return WorkerThread;
        }

        public void Enqueue(ICommand Command)
        {
            lock (SyncRoot)
            {
                CommandQueue.Enqueue(Command);
                Monitor.Pulse(SyncRoot);
            }
        }

        public void RequestHardStop()
        {
            lock (SyncRoot)
            {
                IsHardStopped = true;
                Monitor.PulseAll(SyncRoot);
            }
        }

        private void WorkerLoop()
        {
            while (true)
            {
                ICommand? NewCommand = null;

                lock (SyncRoot)
                {
                    while (CommandQueue.Count == 0 && !Scheduler.HasCommand() && !IsHardStopped)
                    {
                        Monitor.Wait(SyncRoot);
                    }

                    if (IsHardStopped)
                    {
                        return;
                    }

                    if (CommandQueue.Count > 0)
                    {
                        NewCommand = CommandQueue.Dequeue();
                    }
                }

                if (NewCommand != null)
                {
                    Scheduler.Add(NewCommand);
                }

                if (Scheduler.HasCommand())
                {
                    ICommand? CurrentCommand = Scheduler.Select();

                    if (CurrentCommand != null)
                    {
                        try
                        {
                            bool Finished = CurrentCommand.Execute();

                            if (!Finished)
                            {
                                Scheduler.Add(CurrentCommand);
                            }
                        }
                        catch (Exception Ex)
                        {
                            if (Handler != null)
                            {
                                Handler(CurrentCommand, Ex);
                            }
                        }
                    }
                }
            }
        }
    }
}
