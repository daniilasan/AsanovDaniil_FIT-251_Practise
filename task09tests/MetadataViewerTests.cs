using Xunit;
using System.Reflection;
using System;
using System.IO;

namespace task09tests
{
    public class MetadataViewerTests
    {
        [Fact]
        public void FileSystemCommands_HasClassesWithAttributes()
        {
            string DllPath = typeof(FileSystemCommands.DirectorySizeCommand).Assembly.Location;
            Assembly LoadedAssembly = Assembly.LoadFrom(DllPath);
            Type[] Types = LoadedAssembly.GetTypes();

            bool FoundDirectorySizeCommand = false;
            bool FoundFindFilesCommand = false;

            foreach (Type CurrentType in Types)
            {
                if (CurrentType.Name == "DirectorySizeCommand")
                {
                    FoundDirectorySizeCommand = true;
                    object[] Attributes = CurrentType.GetCustomAttributes(false);
                    Assert.True(Attributes.Length > 0);
                }

                if (CurrentType.Name == "FindFilesCommand")
                {
                    FoundFindFilesCommand = true;
                    object[] Attributes = CurrentType.GetCustomAttributes(false);
                    Assert.True(Attributes.Length > 0);
                }
            }

            Assert.True(FoundDirectorySizeCommand);
            Assert.True(FoundFindFilesCommand);
        }

        [Fact]
        public void DirectorySizeCommand_HasConstructorWithParameter()
        {
            Type CommandType = typeof(FileSystemCommands.DirectorySizeCommand);
            ConstructorInfo[] Constructors = CommandType.GetConstructors();

            Assert.True(Constructors.Length > 0);

            ConstructorInfo Constructor = Constructors[0];
            ParameterInfo[] Parameters = Constructor.GetParameters();

            Assert.Single(Parameters);
            Assert.Equal("DirectoryPath", Parameters[0].Name);
            Assert.Equal(typeof(string), Parameters[0].ParameterType);
        }

        [Fact]
        public void FindFilesCommand_HasConstructorWithTwoParameters()
        {
            Type CommandType = typeof(FileSystemCommands.FindFilesCommand);
            ConstructorInfo[] Constructors = CommandType.GetConstructors();

            Assert.True(Constructors.Length > 0);

            ConstructorInfo Constructor = Constructors[0];
            ParameterInfo[] Parameters = Constructor.GetParameters();

            Assert.Equal(2, Parameters.Length);
            Assert.Equal("DirectoryPath", Parameters[0].Name);
            Assert.Equal("FileMask", Parameters[1].Name);
        }

        [Fact]
        public void DirectorySizeCommand_HasExecuteMethod()
        {
            Type CommandType = typeof(FileSystemCommands.DirectorySizeCommand);
            MethodInfo? ExecuteMethod = CommandType.GetMethod("Execute");

            Assert.NotNull(ExecuteMethod);
            Assert.Equal(typeof(void), ExecuteMethod.ReturnType);
        }

        [Fact]
        public void ExecuteMethod_HasDisplayNameAttribute()
        {
            Type CommandType = typeof(FileSystemCommands.DirectorySizeCommand);
            MethodInfo? ExecuteMethod = CommandType.GetMethod("Execute");

            Assert.NotNull(ExecuteMethod);

            object[] Attributes = ExecuteMethod.GetCustomAttributes(false);
            Assert.True(Attributes.Length > 0);
        }
    }
}
