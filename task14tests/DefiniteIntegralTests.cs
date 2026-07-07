using Xunit;
using task14;
using System;

namespace task14tests
{
    public class DefiniteIntegralTests
    {
        private Func<double, double> X = (double x) => x;
        private Func<double, double> SIN = (double x) => Math.Sin(x);

        [Fact]
        public void Solve_IntegralOfX_FromMinusOneToOne_EqualsZero()
        {
            double Result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
            Assert.Equal(0, Result, 3);
        }

        [Fact]
        public void Solve_IntegralOfSin_FromMinusOneToOne_EqualsZero()
        {
            double Result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);

            Assert.Equal(0, Result, 4);
        }

        [Fact]
        public void Solve_IntegralOfX_FromZeroToFive_WithEightThreads()
        {
            double Result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);

            //Интеграл от x на [0,5] = 25/2 = 12.5(в методички неточность в примере этом)
            Assert.Equal(12.5, Result, 3);
        }

        [Fact]
        public void Solve_SingleThread_WorksCorrectly()
        {
            double Result = DefiniteIntegral.Solve(0, 2, X, 1e-4, 1);

            //Интеграл от x на [0,2] = 4/2 = 2
            Assert.Equal(2.0, Result, 3);
        }

        [Fact]
        public void Solve_MultipleThreads_GivesSameResultAsSingleThread()
        {
            double SingleThreadResult = DefiniteIntegral.Solve(0, 3, X, 1e-5, 1);
            double MultiThreadResult = DefiniteIntegral.Solve(0, 3, X, 1e-5, 4);

            Assert.Equal(SingleThreadResult, MultiThreadResult, 4);
        }

        [Fact]
        public void Solve_ConstantFunction_ReturnsCorrectArea()
        {
            Func<double, double> Constant = (double x) => 5.0;

            double Result = DefiniteIntegral.Solve(0, 10, Constant, 1e-3, 4);

            Assert.Equal(50.0, Result, 1);
        }

        [Fact]
        public void Solve_NegativeThreadsNumber_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => DefiniteIntegral.Solve(0, 1, X, 1e-4, -1));
        }

        [Fact]
        public void Solve_ZeroStep_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => DefiniteIntegral.Solve(0, 1, X, 0, 2));
        }

        [Fact]
        public void Solve_NullFunction_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => DefiniteIntegral.Solve(0, 1, null!, 1e-4, 2));
        }



        [Fact]
        public void SolveSingleThread_SinOnMinus100To100_IsCloseToZero()
        {
            Func<double, double> SinFunction = (double X) => Math.Sin(X);

            double Result = DefiniteIntegral.SolveSingleThread(-100, 100, SinFunction, 1e-4);
            double AbsResult = Math.Abs(Result);

            Assert.True(AbsResult < 1e-3, "Result: " + Result);
        }

        [Fact]
        public void SolveSingleThread_MatchesMultiThreadVersion()
        {
            Func<double, double> SinFunction = (double X) => Math.Sin(X);

            double SingleResult = DefiniteIntegral.SolveSingleThread(-100, 100, SinFunction, 1e-4);
            double MultiResult = DefiniteIntegral.Solve(-100, 100, SinFunction, 1e-4, 4);

            Assert.Equal(SingleResult, MultiResult, 6);
        }
    }
}
