using System;
using System.IO;
using System.Reflection;
using CommandLib;
using FileSystemCommands;

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

            //Динамическая загрузка библиотеки через рефлексию
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

                            //Создаём экземпляры с нужными параметрами
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
                                //Выполняем через интерфейс
                                ICommand Command = (ICommand)CommandInstance;
                                Command.Execute();

                                //Приведение типа для получения результата 
                                if (CommandInstance is DirectorySizeCommand SizeCommand)
                                {
                                    Console.WriteLine("Result: total size = " + SizeCommand.TotalSize + " bytes");
                                }
                                else if (CommandInstance is FindFilesCommand FindCommand)
                                {
                                    Console.WriteLine("Result: found " + FindCommand.FoundFiles.Length + " files");
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
