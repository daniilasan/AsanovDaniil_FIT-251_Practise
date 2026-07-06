using System;
using System.Linq;
using System.Reflection;

namespace task05
{
    public class ClassAnalyzer
    {
        private Type AnalyzedType;

        public ClassAnalyzer(Type AnalyzedType)
        {
            if (AnalyzedType == null)
            {
                throw new ArgumentNullException("AnalyzedType", "Type cannot be null");
            }

            this.AnalyzedType = AnalyzedType;
        }

        public string[] GetPublicMethods()
        {
            if (AnalyzedType == null)
            {
                throw new ArgumentNullException("AnalyzedType", "Type cannot be null");
            }

            MethodInfo[] Methods = AnalyzedType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            return Methods.Select(Method => Method.Name).ToArray();
        }

        public string[] GetMethodParams(string MethodName)
        {
            if (AnalyzedType == null)
            {
                throw new ArgumentNullException("AnalyzedType", "Type cannot be null");
            }

            if (string.IsNullOrEmpty(MethodName))
            {
                throw new ArgumentException("MethodName cannot be null or empty", "MethodName");
            }

            MethodInfo? Method = AnalyzedType.GetMethod(MethodName);

            if (Method == null)
            {
                throw new InvalidOperationException("Method not found: " + MethodName);
            }

            string ReturnType = "Return: " + Method.ReturnType.Name;

            string[] Params = Method.GetParameters()
                .Select(Param => Param.ParameterType.Name + " " + Param.Name)
                .ToArray();

            string[] Result = new string[Params.Length + 1];
            Result[0] = ReturnType;

            for (int Index = 0; Index < Params.Length; Index++)
            {
                Result[Index + 1] = Params[Index];
            }

            return Result;
        }

        public string[] GetAllFields()
        {
            if (AnalyzedType == null)
            {
                throw new ArgumentNullException("AnalyzedType", "Type cannot be null");
            }

            FieldInfo[] Fields = AnalyzedType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            return Fields.Select(Field => Field.Name).ToArray();
        }

        public string[] GetProperties()
        {
            if (AnalyzedType == null)
            {
                throw new ArgumentNullException("AnalyzedType", "Type cannot be null");
            }

            PropertyInfo[] Properties = AnalyzedType.GetProperties();

            return Properties.Select(Property => Property.Name).ToArray();
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            if (AnalyzedType == null)
            {
                throw new ArgumentNullException("AnalyzedType", "Type cannot be null");
            }

            return Attribute.IsDefined(AnalyzedType, typeof(T));
        }
    }
}
