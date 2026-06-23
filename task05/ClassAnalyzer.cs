using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task05
{
    public class ClassAnalyzer
    {
        private readonly Type _type;

        public ClassAnalyzer(Type type)
        {
            _type = type;
        }

        public IEnumerable<string> GetPublicMethods()
        {
            MethodInfo[] methods = _type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            IEnumerable<string> methodNames = methods.Select(m => m.Name);
            return methodNames;
        }

        public IEnumerable<string> GetMethodParams(string methodname)
        {
            MethodInfo? method = _type.GetMethod(methodname);

            if (method == null)
            {
                return Enumerable.Empty<string>();
            }

            List<string> result = new List<string>();
            result.Add($"Return: {method.ReturnType.Name}");

            ParameterInfo[] parameters = method.GetParameters();
            IEnumerable<string> paramStrings = parameters.Select(p => $"{p.ParameterType.Name} {p.Name}");
            result.AddRange(paramStrings);

            return result;
        }

        public IEnumerable<string> GetAllFields()
        {
            FieldInfo[] fields = _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            IEnumerable<string> fieldNames = fields.Select(f => f.Name);
            return fieldNames;
        }

        public IEnumerable<string> GetProperties()
        {
            PropertyInfo[] properties = _type.GetProperties();
            IEnumerable<string> propertyNames = properties.Select(p => p.Name);
            return propertyNames;
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            T? attribute = _type.GetCustomAttribute<T>();

            if (attribute != null)
            {
                return true;
            }

            return false;
        }
    }
}
