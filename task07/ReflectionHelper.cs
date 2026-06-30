using System;
using System.Reflection;

namespace task07
{
    public static class ReflectionHelper
    {
        public static void PrintTypeInfo(Type TypeToAnalyze)
        {
            DisplayNameAttribute? DisplayAttr = TypeToAnalyze.GetCustomAttribute<DisplayNameAttribute>();

            if (DisplayAttr != null)
            {
                Console.WriteLine($"Class Display Name: {DisplayAttr.DisplayName}");
            }

            VersionAttribute? VersionAttr = TypeToAnalyze.GetCustomAttribute<VersionAttribute>();

            if (VersionAttr != null)
            {
                Console.WriteLine($"Version: {VersionAttr.Major}.{VersionAttr.Minor}");
            }

            MethodInfo[] Methods = TypeToAnalyze.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (MethodInfo Method in Methods)
            {
                DisplayNameAttribute? MethodDisplayAttr = Method.GetCustomAttribute<DisplayNameAttribute>();

                if (MethodDisplayAttr != null)
                {
                    Console.WriteLine($"Method: {Method.Name} - Display Name: {MethodDisplayAttr.DisplayName}");
                }
            }

            PropertyInfo[] Properties = TypeToAnalyze.GetProperties();

            foreach (PropertyInfo Property in Properties)
            {
                DisplayNameAttribute? PropDisplayAttr = Property.GetCustomAttribute<DisplayNameAttribute>();

                if (PropDisplayAttr != null)
                {
                    Console.WriteLine($"Property: {Property.Name} - Display Name: {PropDisplayAttr.DisplayName}");
                }
            }
        }
    }
}
