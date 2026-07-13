using System.Collections.Generic;

namespace task17
{
    public class RoundRobinScheduler : IScheduler
    {
        private List<ICommand> Commands;

        public RoundRobinScheduler()
        {
            Commands = new List<ICommand>();
        }

        public bool HasCommand()
        {
            return Commands.Count > 0;
        }

        public ICommand? Select()
        {
            if (Commands.Count == 0)
            {
                return null;
            }

            ICommand Command = Commands[0];
            Commands.RemoveAt(0);
            return Command;
        }

        public void Add(ICommand Command)
        {
            Commands.Add(Command);
        }
    }
}
