using System;
using System.Threading;

namespace task14
{
    public class DefiniteIntegral
    {
        public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
        {
            if (threadsNumber <= 0)
            {
                throw new ArgumentException("threadsNumber must be positive", "threadsNumber");
            }

            if (step <= 0)
            {
                throw new ArgumentException("step must be positive", "step");
            }

            if (function == null)
            {
                throw new ArgumentNullException("function", "function cannot be null");
            }

            //переменная для накопления результата
            double SharedResult = 0.0;

            //Барьер для синхронизации: threadsNumber потоков + 1 основной поток
            Barrier Barrier = new Barrier(threadsNumber + 1);

            double SegmentLength = (b - a) / threadsNumber;
            Thread[] Threads = new Thread[threadsNumber];

            for (int Index = 0; Index < threadsNumber; Index++)
            {
                int ThreadIndex = Index;
                double SegmentStart = a + ThreadIndex * SegmentLength;
                double SegmentEnd = a + (ThreadIndex + 1) * SegmentLength;

                Threads[Index] = new Thread(() =>
                {
                    //Вычисляем интеграл
                    double PartialResult = CalculateIntegral(SegmentStart, SegmentEnd, function, step);

                    //Потокобезопасное сложение через Interlocked.CompareExchange
                    //(Interlocked.Add не работает с double, поэтому используем этот трюк)
                    double OriginalValue;
                    double NewValue;

                    do
                    {
                        OriginalValue = SharedResult;
                        NewValue = OriginalValue + PartialResult;
                    }
                    while (Interlocked.CompareExchange(ref SharedResult, NewValue, OriginalValue) != OriginalValue);

                    //Сигналим барьеру, что этот поток закончил работу
                    Barrier.SignalAndWait();
                });

                Threads[Index].Start();
            }

            //Основной поток тоже ждёт на барьере, пока все рабочие потоки не завершатся
            Barrier.SignalAndWait();

            return SharedResult;
        }

        private static double CalculateIntegral(double a, double b, Func<double, double> function, double step)
        {
            double Result = 0.0;
            double CurrentX = a;

            while (CurrentX < b)
            {
                double NextX = CurrentX + step;
                if (NextX > b)
                {
                    NextX = b;
                }

                double Y1 = function(CurrentX);
                double Y2 = function(NextX);
                double TrapezoidArea = (Y1 + Y2) * step / 2.0;

                Result = Result + TrapezoidArea;
                CurrentX = NextX;
            }

            return Result;
        }


        public static double SolveSingleThread(double A, double B, Func<double, double> Function, double Step)
        {
            if (Step <= 0)
            {
                throw new ArgumentException("step must be positive", "step");
            }

            if (Function == null)
            {
                throw new ArgumentNullException("function", "function cannot be null");
            }

            double Result = 0.0;
            double CurrentX = A;

            while (CurrentX < B)
            {
                double NextX = CurrentX + Step;

                if (NextX > B)
                {
                    NextX = B;
                }

                double Y1 = Function(CurrentX);
                double Y2 = Function(NextX);
                double TrapezoidArea = (Y1 + Y2) * Step / 2.0;

                Result = Result + TrapezoidArea;
                CurrentX = NextX;
            }

            return Result;
        }
    }
}
