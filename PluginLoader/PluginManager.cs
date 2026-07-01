using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using CommandLib;

namespace PluginLoader
{
    public class PluginManager
    {
        private string PluginsDirectory;
        private List<ICommand> LoadedPlugins;

        public PluginManager(string PluginsDirectory)
        {
            this.PluginsDirectory = PluginsDirectory;
            this.LoadedPlugins = new List<ICommand>();
        }

        public void LoadAndExecutePlugins()
        {
            if (Directory.Exists(PluginsDirectory) == false)
            {
                Console.WriteLine("Plugins directory not found: " + PluginsDirectory);
                return;
            }

            string[] DllFiles = Directory.GetFiles(PluginsDirectory, "*.dll");
            List<PluginInfo> PluginInfos = new List<PluginInfo>();

            //Загружаем все DLL и собираем информацию о плагинах
            foreach (string DllFile in DllFiles)
            {
                try
                {
                    Assembly LoadedAssembly = Assembly.LoadFrom(DllFile);
                    Type[] Types = LoadedAssembly.GetTypes();

                    foreach (Type CurrentType in Types)
                    {
                        if (CurrentType.IsClass == true && CurrentType.IsAbstract == false)
                        {
                            PluginLoadAttribute? PluginAttr = CurrentType.GetCustomAttribute<PluginLoadAttribute>();

                            if (PluginAttr != null)
                            {
                                Type[] Interfaces = CurrentType.GetInterfaces();
                                bool ImplementsICommand = false;

                                foreach (Type Interface in Interfaces)
                                {
                                    if (Interface == typeof(ICommand))
                                    {
                                        ImplementsICommand = true;
                                        break;
                                    }
                                }

                                if (ImplementsICommand == true)
                                {
                                    PluginInfo Info = new PluginInfo();
                                    Info.PluginType = CurrentType;
                                    Info.PluginName = PluginAttr.PluginName;
                                    Info.Dependencies = PluginAttr.Dependencies;

                                    PluginInfos.Add(Info);
                                }
                            }
                        }
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Error loading " + DllFile + ": " + Ex.Message);
                }
            }

            //Сортируем плагины по зависимостям (топологическая сортировка)
            List<PluginInfo> SortedPlugins = TopologicalSort(PluginInfos);

            //Создаём экземпляры и выполняем
            foreach (PluginInfo Plugin in SortedPlugins)
            {
                try
                {
                    Console.WriteLine("Loading plugin: " + Plugin.PluginName);

                    object? Instance = Activator.CreateInstance(Plugin.PluginType);

                    if (Instance != null)
                    {
                        ICommand Command = (ICommand)Instance;
                        LoadedPlugins.Add(Command);

                        Console.WriteLine("Executing plugin: " + Plugin.PluginName);
                        Command.Execute();
                        Console.WriteLine();
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine("Error executing plugin " + Plugin.PluginName + ": " + Ex.Message);
                }
            }
        }

        private List<PluginInfo> TopologicalSort(List<PluginInfo> Plugins)
        {
            List<PluginInfo> Sorted = new List<PluginInfo>();
            HashSet<string> Visited = new HashSet<string>();
            HashSet<string> InProgress = new HashSet<string>();

            foreach (PluginInfo Plugin in Plugins)
            {
                if (Visited.Contains(Plugin.PluginName) == false)
                {
                    Visit(Plugin, Plugins, Sorted, Visited, InProgress);
                }
            }

            return Sorted;
        }

        private void Visit(PluginInfo Plugin, List<PluginInfo> AllPlugins, List<PluginInfo> Sorted, HashSet<string> Visited, HashSet<string> InProgress)
        {
            if (InProgress.Contains(Plugin.PluginName) == true)
            {
                Console.WriteLine("Circular dependency detected for plugin: " + Plugin.PluginName);
                return;
            }

            if (Visited.Contains(Plugin.PluginName) == true)
            {
                return;
            }

            InProgress.Add(Plugin.PluginName);

            foreach (string Dependency in Plugin.Dependencies)
            {
                PluginInfo? DependencyPlugin = null;

                foreach (PluginInfo P in AllPlugins)
                {
                    if (P.PluginName == Dependency)
                    {
                        DependencyPlugin = P;
                        break;
                    }
                }

                if (DependencyPlugin != null)
                {
                    Visit(DependencyPlugin, AllPlugins, Sorted, Visited, InProgress);
                }
            }

            InProgress.Remove(Plugin.PluginName);
            Visited.Add(Plugin.PluginName);
            Sorted.Add(Plugin);
        }

        public List<ICommand> GetLoadedPlugins()
        {
            return LoadedPlugins;
        }
    }

    internal class PluginInfo
    {
        public Type PluginType { get; set; } = typeof(object);
        public string PluginName { get; set; } = "";
        public string[] Dependencies { get; set; } = new string[0];
    }//Задаём так,чтобы подавит предупреждения CI
}
