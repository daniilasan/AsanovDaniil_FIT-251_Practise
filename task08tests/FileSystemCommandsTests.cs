using Xunit;
using FileSystemCommands;
using System.IO;
using System;

namespace task08tests
{
    public class FileSystemCommandsTests
    {
        [Fact]
        public void DirectorySizeCommand_ShouldCalculateSize()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "TestDir_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(TestDir);

            File.WriteAllText(Path.Combine(TestDir, "test1.txt"), "Hello");
            File.WriteAllText(Path.Combine(TestDir, "test2.txt"), "World");

            DirectorySizeCommand Command = new DirectorySizeCommand(TestDir);
            Command.Execute();
            Assert.True(Command.TotalSize > 0);

            Directory.Delete(TestDir, true);
        }

        [Fact]
        public void FindFilesCommand_ShouldFindMatchingFiles()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "TestDir_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(TestDir);

            File.WriteAllText(Path.Combine(TestDir, "file1.txt"), "Text");
            File.WriteAllText(Path.Combine(TestDir, "file2.log"), "Log");
            File.WriteAllText(Path.Combine(TestDir, "file3.txt"), "More text");

            FindFilesCommand Command = new FindFilesCommand(TestDir, "*.txt");
            Command.Execute();
            Assert.Equal(2, Command.FoundFiles.Length);

            Directory.Delete(TestDir, true);
        }

        [Fact]
        public void DirectorySizeCommand_EmptyDirectory_ReturnsZero()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "EmptyDir_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(TestDir);

            DirectorySizeCommand Command = new DirectorySizeCommand(TestDir);
            Command.Execute();

            Assert.Equal(0, Command.TotalSize);

            Directory.Delete(TestDir, true);
        }

        [Fact]
        public void FindFilesCommand_NoMatchingFiles_ReturnsEmpty()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "TestDir_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(TestDir);

            File.WriteAllText(Path.Combine(TestDir, "file.log"), "Log");

            FindFilesCommand Command = new FindFilesCommand(TestDir, "*.txt");
            Command.Execute();
            Assert.Empty(Command.FoundFiles);

            Directory.Delete(TestDir, true);
        }

        [Fact]
        public void DirectorySizeCommand_WithSubdirectories_ShouldIncludeNestedFiles()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "TestDir_" + Guid.NewGuid().ToString());
            string SubDir = Path.Combine(TestDir, "SubDir");
            Directory.CreateDirectory(TestDir);
            Directory.CreateDirectory(SubDir);

            File.WriteAllText(Path.Combine(TestDir, "root.txt"), "Root file content");
            File.WriteAllText(Path.Combine(SubDir, "nested.txt"), "Nested file content");

            DirectorySizeCommand Command = new DirectorySizeCommand(TestDir);
            Command.Execute();
            long ExpectedMinSize = "Root file content".Length + "Nested file content".Length;
            Assert.True(Command.TotalSize >= ExpectedMinSize);

            Directory.Delete(TestDir, true);
        }

        [Fact]
        public void FindFilesCommand_WithSubdirectories_ShouldFindNestedFiles()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "TestDir_" + Guid.NewGuid().ToString());
            string SubDir = Path.Combine(TestDir, "SubDir");
            Directory.CreateDirectory(TestDir);
            Directory.CreateDirectory(SubDir);

            File.WriteAllText(Path.Combine(TestDir, "root.txt"), "Root");
            File.WriteAllText(Path.Combine(SubDir, "nested.txt"), "Nested");
            File.WriteAllText(Path.Combine(SubDir, "other.log"), "Log");

            FindFilesCommand Command = new FindFilesCommand(TestDir, "*.txt");
            Command.Execute();
            Assert.Equal(2, Command.FoundFiles.Length);

            Directory.Delete(TestDir, true);
        }
    }
}
