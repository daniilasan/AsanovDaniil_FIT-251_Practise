using System;
using System.IO;
using System.Reflection;

namespace MetadataViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: MetadataViewer <path-to-dll>");
                return;
            }

            string DllPath = args[0];

            if (File.Exists(DllPath) == false)
            {
                Console.WriteLine("File not found: " + DllPath);
                return;
            }

            Assembly LoadedAssembly = Assembly.LoadFrom(DllPath);
            Type[] Types = LoadedAssembly.GetTypes();

            Console.WriteLine("=== Assembly Metadata ===");
            Console.WriteLine("Assembly: " + LoadedAssembly.FullName);
            Console.WriteLine();

            foreach (Type CurrentType in Types)
            {
                if (CurrentType.IsClass == false)
                {
                    continue;
                }

                Console.WriteLine("--- Class: " + CurrentType.Name + " ---");
                Console.WriteLine("Full Name: " + CurrentType.FullName);
                Console.WriteLine("Namespace: " + CurrentType.Namespace);
                Console.WriteLine();

                //Атрибуты класса
                object[] ClassAttributes = CurrentType.GetCustomAttributes(false);
                if (ClassAttributes.Length > 0)
                {
                    Console.WriteLine("Attributes:");
                    foreach (object Attribute in ClassAttributes)
                    {
                        Console.WriteLine("  - " + Attribute.GetType().Name);
                    }
                    Console.WriteLine();
                }

                //Конструкторы
                ConstructorInfo[] Constructors = CurrentType.GetConstructors();
                if (Constructors.Length > 0)
                {
                    Console.WriteLine("Constructors:");
                    foreach (ConstructorInfo Constructor in Constructors)
                    {
                        ParameterInfo[] Parameters = Constructor.GetParameters();
                        string ParamList = "";

                        foreach (ParameterInfo Param in Parameters)
                        {
                            if (ParamList.Length > 0)
                            {
                                ParamList = ParamList + ", ";
                            }
                            ParamList = ParamList + Param.ParameterType.Name + " " + Param.Name;
                        }

                        Console.WriteLine(" - " + Constructor.Name + "(" + ParamList + ")");
                    }
                    Console.WriteLine();
                }

                //Методы
                MethodInfo[] Methods = CurrentType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (Methods.Length > 0)
                {
                    Console.WriteLine("Methods:");
                    foreach (MethodInfo Method in Methods)
                    {
                        ParameterInfo[] Parameters = Method.GetParameters();
                        string ParamList = "";

                        foreach (ParameterInfo Param in Parameters)
                        {
                            if (ParamList.Length > 0)
                            {
                                ParamList = ParamList + ", ";
                            }
                            ParamList = ParamList + Param.ParameterType.Name + " " + Param.Name;
                        }

                        Console.WriteLine("  - " + Method.ReturnType.Name + " " + Method.Name + "(" + ParamList + ")");

                        //Атрибуты метода
                        object[] MethodAttributes = Method.GetCustomAttributes(false);
                        if (MethodAttributes.Length > 0)
                        {
                            Console.WriteLine("Attributes:");
                            foreach (object Attribute in MethodAttributes)
                            {
                                Console.WriteLine(" - " + Attribute.GetType().Name);
                            }
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }
    }
}
