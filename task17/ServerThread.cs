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
        private bool IsStopped;
        private int Quantum;
        private ExceptionHandler? Handler;

        public ServerThread(ExceptionHandler? Handler, IScheduler Scheduler, int Quantum)
        {
            this.Handler = Handler;
            this.Scheduler = Scheduler;
            this.Quantum = Quantum;
            CommandQueue = new Queue<ICommand>();
            SyncRoot = new object();
            IsStopped = false;

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

        public void Stop()
        {
            lock (SyncRoot)
            {
                IsStopped = true;
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
                    while (CommandQueue.Count == 0 && !Scheduler.HasCommand() && !IsStopped)
                    {
                        Monitor.Wait(SyncRoot);
                    }

                    if (IsStopped)
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
