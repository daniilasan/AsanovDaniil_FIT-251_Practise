using Xunit;
using task05;
using System.Linq;

namespace task05tests
{
    public class TestClass
    {
        public int PublicField;

        private string _privateField;


        public int Property { get; set; }

        public void Method()
        {
        }

        public int Add(int a, int b)
        {
            return a + b;
        }
    }

    [System.Serializable]
    public class AttributedClass
    {
    }

    public class ClassAnalyzerTests
    {
        
    
        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            ClassAnalyzer analyzer = new ClassAnalyzer(typeof(TestClass));
            IEnumerable<string> methods = analyzer.GetPublicMethods();

            Assert.Contains("Method", methods);
            Assert.Contains("Add", methods);
        }

        [Fact]
        public void GetAllFields_IncludesPrivateFields()
        {
            ClassAnalyzer analyzer = new ClassAnalyzer(typeof(TestClass));
            IEnumerable<string> fields = analyzer.GetAllFields();

            Assert.Contains("_privateField", fields);
            Assert.Contains("PublicField", fields);
        }

        [Fact]
        public void GetProperties_ReturnsCorrectProperties()
        {
            ClassAnalyzer analyzer = new ClassAnalyzer(typeof(TestClass));
            IEnumerable<string> properties = analyzer.GetProperties();

            Assert.Single(properties);
            Assert.Contains("Property", properties);
        }

        [Fact]
        public void GetMethodParams_ReturnsCorrectParams()
        {
            ClassAnalyzer analyzer = new ClassAnalyzer(typeof(TestClass));
            IEnumerable<string> methodParams = analyzer.GetMethodParams("Add");

            Assert.Equal("Return: Int32", methodParams.ElementAt(0));
            Assert.Equal("Int32 a", methodParams.ElementAt(1));
            Assert.Equal("Int32 b", methodParams.ElementAt(2));
        }

        [Fact]
        public void HasAttribute_ReturnsTrue_WhenAttributeExists()
        {
            ClassAnalyzer analyzer = new ClassAnalyzer(typeof(AttributedClass));
            bool hasAttribute = analyzer.HasAttribute<System.SerializableAttribute>();

            Assert.True(hasAttribute);
        }

        [Fact]
        public void HasAttribute_ReturnsFalse_WhenNoAttribute()
        {
            ClassAnalyzer analyzer = new ClassAnalyzer(typeof(TestClass));
            bool hasAttribute = analyzer.HasAttribute<System.SerializableAttribute>();

            Assert.False(hasAttribute);
        }
    }
}
