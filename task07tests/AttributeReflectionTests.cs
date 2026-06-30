using Xunit;
using task07;
using System.Reflection;
using System.IO;

namespace task07tests
{
    public class AttributeReflectionTests
    {
        [Fact]
        public void Class_HasDisplayNameAttribute()
        {
            Type Type = typeof(SampleClass);
            DisplayNameAttribute? Attribute = Type.GetCustomAttribute<DisplayNameAttribute>();

            Assert.NotNull(Attribute);
            Assert.Equal("Пример класса", Attribute.DisplayName);
        }

        [Fact]
        public void Method_HasDisplayNameAttribute()
        {
            MethodInfo? Method = typeof(SampleClass).GetMethod("TestMethod");

            Assert.NotNull(Method);

            DisplayNameAttribute? Attribute = Method.GetCustomAttribute<DisplayNameAttribute>();

            Assert.NotNull(Attribute);
            Assert.Equal("Тестовый метод", Attribute.DisplayName);
        }

        [Fact]
        public void Property_HasDisplayNameAttribute()
        {
            PropertyInfo? Property = typeof(SampleClass).GetProperty("Number");

            Assert.NotNull(Property);

            DisplayNameAttribute? Attribute = Property.GetCustomAttribute<DisplayNameAttribute>();

            Assert.NotNull(Attribute);
            Assert.Equal("Числовое свойство", Attribute.DisplayName);
        }

        [Fact]
        public void Class_HasVersionAttribute()
        {
            Type Type = typeof(SampleClass);
            VersionAttribute? Attribute = Type.GetCustomAttribute<VersionAttribute>();

            Assert.NotNull(Attribute);
            Assert.Equal(1, Attribute.Major);
            Assert.Equal(0, Attribute.Minor);
        }

        [Fact]
        public void PrintTypeInfo_OutputsCorrectInformation()
        {
            StringWriter Output = new StringWriter();
            Console.SetOut(Output);

            ReflectionHelper.PrintTypeInfo(typeof(SampleClass));

            string Result = Output.ToString();

            Assert.Contains("Class Display Name: Пример класса", Result);
            Assert.Contains("Version: 1.0", Result);
            Assert.Contains("Method: TestMethod - Display Name: Тестовый метод", Result);
            Assert.Contains("Property: Number - Display Name: Числовое свойство", Result);
        }
    }
}
