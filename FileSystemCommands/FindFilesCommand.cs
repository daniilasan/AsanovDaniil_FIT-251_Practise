using System;
using System.IO;
using CommandLib;

namespace FileSystemCommands
{
    [DisplayName("Команда поиска файлов по маске")]
    [Version(1, 0)]
    public class FindFilesCommand : ICommand
    {
        private string DirectoryPath;
        private string FileMask;

        public string[] FoundFiles { get; private set; }

        public FindFilesCommand(string DirectoryPath, string FileMask)
        {
            this.DirectoryPath = DirectoryPath;
            this.FileMask = FileMask;
            this.FoundFiles = new string[0];
        }

        [DisplayName("Найти файлы по маске")]
        public void Execute()
        {
            if (Directory.Exists(DirectoryPath) == false)
            {
                Console.WriteLine("Directory not found: " + DirectoryPath);
                return;
            }

            FoundFiles = Directory.GetFiles(DirectoryPath, FileMask, SearchOption.AllDirectories);

            Console.WriteLine("Found " + FoundFiles.Length + " files:");

            foreach (string File in FoundFiles)
            {
                Console.WriteLine("  " + File);
            }
        }
    }
}
