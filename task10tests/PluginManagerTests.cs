using Xunit;
using PluginLoader;
using System.IO;
using System;
using System.Reflection;

namespace task10tests
{
    public class PluginManagerTests
    {
        [Fact]
        public void PluginManager_LoadsPluginsFromDirectory()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "Plugins_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(TestDir);

            string PluginADll = typeof(TestPluginA.PluginA).Assembly.Location;
            string PluginBDll = typeof(TestPluginB.PluginB).Assembly.Location;

            File.Copy(PluginADll, Path.Combine(TestDir, "TestPluginA.dll"));
            File.Copy(PluginBDll, Path.Combine(TestDir, "TestPluginB.dll"));

            PluginManager Manager = new PluginManager(TestDir);
            Manager.LoadAndExecutePlugins();

            var LoadedPlugins = Manager.GetLoadedPlugins();
            Assert.True(LoadedPlugins.Count >= 2);

            Directory.Delete(TestDir, true);
        }

        [Fact]
        public void PluginManager_RespectsDependencies()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "Plugins_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(TestDir);

            string PluginADll = typeof(TestPluginA.PluginA).Assembly.Location;
            string PluginBDll = typeof(TestPluginB.PluginB).Assembly.Location;

            File.Copy(PluginADll, Path.Combine(TestDir, "TestPluginA.dll"));
            File.Copy(PluginBDll, Path.Combine(TestDir, "TestPluginB.dll"));

            PluginManager Manager = new PluginManager(TestDir);
            Manager.LoadAndExecutePlugins();

            var LoadedPlugins = Manager.GetLoadedPlugins();

            // PluginA должен быть загружен раньше PluginB
            int IndexA = -1;
            int IndexB = -1;

            for (int i = 0; i < LoadedPlugins.Count; i++)
            {
                if (LoadedPlugins[i].GetType().Name == "PluginA")
                {
                    IndexA = i;
                }
                if (LoadedPlugins[i].GetType().Name == "PluginB")
                {
                    IndexB = i;
                }
            }

            if (IndexA >= 0 && IndexB >= 0)
            {
                Assert.True(IndexA < IndexB);
            }

            Directory.Delete(TestDir, true);
        }

        [Fact]
        public void PluginManager_HandlesEmptyDirectory()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "EmptyPlugins_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(TestDir);

            PluginManager Manager = new PluginManager(TestDir);
            Manager.LoadAndExecutePlugins();

            var LoadedPlugins = Manager.GetLoadedPlugins();
            Assert.Empty(LoadedPlugins);

            Directory.Delete(TestDir, true);
        }

        [Fact]
        public void PluginManager_HandlesNonExistentDirectory()
        {
            string TestDir = Path.Combine(Path.GetTempPath(), "NonExistent_" + Guid.NewGuid().ToString());

            PluginManager Manager = new PluginManager(TestDir);
            Manager.LoadAndExecutePlugins();

            var LoadedPlugins = Manager.GetLoadedPlugins();
            Assert.Empty(LoadedPlugins);
        }

        [Fact]
        public void PluginLoadAttribute_StoresPluginName()
        {
            Type PluginType = typeof(TestPluginA.PluginA);
            var Attribute = PluginType.GetCustomAttribute<CommandLib.PluginLoadAttribute>();

            Assert.NotNull(Attribute);
            Assert.Equal("PluginA", Attribute.PluginName);
        }

        [Fact]
        public void PluginLoadAttribute_StoresDependencies()
        {
            Type PluginType = typeof(TestPluginB.PluginB);
            var Attribute = PluginType.GetCustomAttribute<CommandLib.PluginLoadAttribute>();

            Assert.NotNull(Attribute);
            Assert.Single(Attribute.Dependencies);
            Assert.Equal("PluginA", Attribute.Dependencies[0]);
        }
    }
}
