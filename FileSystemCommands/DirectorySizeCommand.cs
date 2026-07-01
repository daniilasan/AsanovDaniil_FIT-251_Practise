using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    [DisplayName("Команда вычисления размера каталога")]
    [Version(1, 0)]
    public class DirectorySizeCommand : ICommand
    {
        private string DirectoryPath;

        public long TotalSize { get; private set; }

        public DirectorySizeCommand(string DirectoryPath)
        {
            this.DirectoryPath = DirectoryPath;
            this.TotalSize = 0;
        }

        [DisplayName("Вычислить размер каталога")]
        public void Execute()
        {
            if (Directory.Exists(DirectoryPath) == false)
            {
                Console.WriteLine("Directory not found: " + DirectoryPath);
                return;
            }

            TotalSize = 0;
            string[] Files = Directory.GetFiles(DirectoryPath, "*.*", SearchOption.AllDirectories);

            foreach (string File in Files)
            {
                FileInfo Info = new FileInfo(File);
                TotalSize = TotalSize + Info.Length;
            }

            Console.WriteLine("Directory size: " + TotalSize + " bytes");
        }
    }
}
