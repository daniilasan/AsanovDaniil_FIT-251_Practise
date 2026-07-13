using System;
using System.Collections.Generic;
using System.Threading;

namespace task17
{
    public class ServerThread
    {
        private Thread WorkerThread;
        private Queue<ICommand> CommandQueue;
        private object SyncRoot;
        private bool IsSoftStopped;
        private bool IsHardStopped;
        private ExceptionHandler? Handler;

        public ServerThread(ExceptionHandler? Handler)
        {
            this.Handler = Handler;
            CommandQueue = new Queue<ICommand>();
            SyncRoot = new object();
            IsSoftStopped = false;
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

        public void RequestSoftStop()
        {
            lock (SyncRoot)
            {
                IsSoftStopped = true;
                Monitor.PulseAll(SyncRoot);
            }
        }

        private void WorkerLoop()
        {
            while (true)
            {
                ICommand? CurrentCommand = null;

                lock (SyncRoot)
                {
                    while (CommandQueue.Count == 0 && !IsSoftStopped && !IsHardStopped)
                    {
                        Monitor.Wait(SyncRoot);
                    }

                    if (IsHardStopped)
                    {
                        return;
                    }

                    if (IsSoftStopped && CommandQueue.Count == 0)
                    {
                        return;
                    }

                    if (CommandQueue.Count > 0)
                    {
                        CurrentCommand = CommandQueue.Dequeue();
                    }
                }

                if (CurrentCommand != null)
                {
                    try
                    {
                        CurrentCommand.Execute();
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
