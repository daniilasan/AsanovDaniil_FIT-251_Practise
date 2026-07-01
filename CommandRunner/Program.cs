using System;
using System.IO;
using System.Reflection;
using CommandLib;

namespace CommandRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            string DllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileSystemCommands.dll");

            if (File.Exists(DllPath) == false)
            {
                Console.WriteLine("DLL not found: " + DllPath);
                return;
            }

            Assembly LoadedAssembly = Assembly.LoadFrom(DllPath);
            Type[] Types = LoadedAssembly.GetTypes();

            foreach (Type CurrentType in Types)
            {
                if (CurrentType.IsClass == true && CurrentType.IsAbstract == false)
                {
                    Type[] Interfaces = CurrentType.GetInterfaces();

                    foreach (Type Interface in Interfaces)
                    {
                        if (Interface == typeof(ICommand))
                        {
                            Console.WriteLine("=== Found command: " + CurrentType.Name + " ===");

                            object? CommandInstance = null;

                            if (CurrentType.Name == "DirectorySizeCommand")
                            {
                                string TestPath = Path.GetTempPath();
                                CommandInstance = Activator.CreateInstance(CurrentType, TestPath);
                            }
                            else if (CurrentType.Name == "FindFilesCommand")
                            {
                                string TestPath = Path.GetTempPath();
                                string Mask = "*.txt";
                                CommandInstance = Activator.CreateInstance(CurrentType, TestPath, Mask);
                            }

                            if (CommandInstance != null)
                            {
                                ICommand Command = (ICommand)CommandInstance;
                                Command.Execute();

                                PropertyInfo? TotalSizeProperty = CurrentType.GetProperty("TotalSize");
                                if (TotalSizeProperty != null)
                                {
                                    object? SizeValue = TotalSizeProperty.GetValue(CommandInstance);
                                    Console.WriteLine("Result: total size = " + SizeValue + " bytes");
                                }

                                PropertyInfo? FoundFilesProperty = CurrentType.GetProperty("FoundFiles");
                                if (FoundFilesProperty != null)
                                {
                                    object? FilesValue = FoundFilesProperty.GetValue(CommandInstance);
                                    if (FilesValue is string[] FilesArray)
                                    {
                                        Console.WriteLine("Result: found " + FilesArray.Length + " files");
                                    }
                                }

                                Console.WriteLine();
                            }

                            break;
                        }
                    }
                }
            }
        }
    }
}

