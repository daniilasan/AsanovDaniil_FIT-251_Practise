using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorGeneratorTests
    {
        private string CalculatorCode;

        public CalculatorGeneratorTests()
        {
            //Строка с исходным кодом, который будет компилироваться
            CalculatorCode = @"
public class Calculator
{
    public int Add(int a, int b) 
    { 
        return a + b; 
    }
    
    public int Minus(int a, int b) 
    { 
        return a - b; 
    }
    
    public int Mul(int a, int b) 
    { 
        return a * b; 
    }
    
    public int Div(int a, int b) 
    { 
        return a / b; 
    }
}";
        }

        [Fact]
        public void Add_ReturnsCorrectSum()
        {
            ClassGenerator Generator = new ClassGenerator();
            ICalculator Calculator = Generator.CreateCalculator(CalculatorCode);

            int FirstNumber = 2;
            int SecondNumber = 3;
            int ExpectedResult = 5;

            int ActualResult = Calculator.Add(FirstNumber, SecondNumber);

            Assert.Equal(ExpectedResult, ActualResult);
        }

        [Fact]
        public void Minus_ReturnsCorrectDifference()
        {
            ClassGenerator Generator = new ClassGenerator();
            ICalculator Calculator = Generator.CreateCalculator(CalculatorCode);

            int FirstNumber = 10;
            int SecondNumber = 4;
            int ExpectedResult = 6;

            int ActualResult = Calculator.Minus(FirstNumber, SecondNumber);

            Assert.Equal(ExpectedResult, ActualResult);
        }

        [Fact]
        public void Mul_ReturnsCorrectProduct()
        {
            ClassGenerator Generator = new ClassGenerator();
            ICalculator Calculator = Generator.CreateCalculator(CalculatorCode);

            int FirstNumber = 3;
            int SecondNumber = 7;
            int ExpectedResult = 21;

            int ActualResult = Calculator.Mul(FirstNumber, SecondNumber);

            Assert.Equal(ExpectedResult, ActualResult);
        }

        [Fact]
        public void Div_ReturnsCorrectQuotient()
        {
            ClassGenerator Generator = new ClassGenerator();
            ICalculator Calculator = Generator.CreateCalculator(CalculatorCode);

            int FirstNumber = 20;
            int SecondNumber = 4;
            int ExpectedResult = 5;

            int ActualResult = Calculator.Div(FirstNumber, SecondNumber);

            Assert.Equal(ExpectedResult, ActualResult);
        }

        [Fact]
        public void GeneratedClass_ImplementsICalculator()
        {
            ClassGenerator Generator = new ClassGenerator();

            ICalculator Calculator = Generator.CreateCalculator(CalculatorCode);

            Assert.NotNull(Calculator);
            Assert.IsAssignableFrom<ICalculator>(Calculator);
        }

        [Fact]
        public void AllOperations_WorkCorrectly()
        {
            ClassGenerator Generator = new ClassGenerator();
            ICalculator Calculator = Generator.CreateCalculator(CalculatorCode);

            int AddResult = Calculator.Add(5, 3);
            int MinusResult = Calculator.Minus(5, 3);
            int MulResult = Calculator.Mul(5, 3);
            int DivResult = Calculator.Div(15, 3);

            Assert.Equal(8, AddResult);
            Assert.Equal(2, MinusResult);
            Assert.Equal(15, MulResult);
            Assert.Equal(5, DivResult);
        }
    }
}
