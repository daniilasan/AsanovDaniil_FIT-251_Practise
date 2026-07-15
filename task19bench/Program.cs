using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using task17;
using ScottPlot;

class Program
{
    private class TestCommand : ICommand
    {
        private int Id;
        private int Counter;
        private int MaxCalls;
        private CountdownEvent? DoneSignal;
        private bool Print;

        public TestCommand(int Id, int MaxCalls, CountdownEvent? DoneSignal, bool Print)
        {
            this.Id = Id;
            this.Counter = 0;
            this.MaxCalls = MaxCalls;
            this.DoneSignal = DoneSignal;
            this.Print = Print;
        }

        public bool Execute()
        {
            Counter = Counter + 1;

            if (Print)
            {
                Console.WriteLine("Поток " + Id + " вызов " + Counter);
            }
            Thread.Sleep(2);

            if (Counter >= MaxCalls)
            {
                if (DoneSignal != null)
                {
                    DoneSignal.Signal();
                }
                return true;
            }
            return false;
        }
    }

    static void Main()
    {
        StringWriter StringOutput = new StringWriter();
        Console.SetOut(StringOutput);

        RoundRobinScheduler Scheduler1 = new RoundRobinScheduler();
        ServerThread Server1 = new ServerThread(null, Scheduler1);

        for (int i = 1; i <= 5; i++)
        {
            Server1.Enqueue(new TestCommand(i, 3, null, true));
        }

        Thread.Sleep(1000);
        Server1.Enqueue(new HardStopCommand(Server1));
        Server1.GetThread().Join();

        StreamWriter StandardOutput = new StreamWriter(Console.OpenStandardOutput());
        StandardOutput.AutoFlush = true;
        Console.SetOut(StandardOutput);

        string LogText = StringOutput.ToString();
        int[] TicksArray = new int[] { 5, 10, 15, 20 };
        List<double> AverageTimes = new List<double>();
        int RunsCount = 3;

        foreach (int Ticks in TicksArray)
        {
            double TotalTime = 0.0;

            for (int run = 0; run < RunsCount; run++)
            {
                RoundRobinScheduler Scheduler2 = new RoundRobinScheduler();
                ServerThread Server2 = new ServerThread(null, Scheduler2);
                CountdownEvent Done = new CountdownEvent(5);

                for (int i = 1; i <= 5; i++)
                {
                    Server2.Enqueue(new TestCommand(i, Ticks, Done, false));
                }

                System.Diagnostics.Stopwatch Timer = System.Diagnostics.Stopwatch.StartNew();

                Done.WaitHandle.WaitOne();

                Timer.Stop();
                TotalTime = TotalTime + Timer.Elapsed.TotalMilliseconds;

                Server2.RequestHardStop();
                Server2.GetThread().Join();
            }

            double AverageTime = TotalTime / RunsCount;
            AverageTimes.Add(AverageTime);
        }
        Plot MyPlot = new Plot();
        double[] X = new double[TicksArray.Length];
        double[] Y = new double[TicksArray.Length];
        for (int i = 0; i < TicksArray.Length; i++)
        {
            X[i] = TicksArray[i];
            Y[i] = AverageTimes[i];
        }

        MyPlot.Add.Scatter(X, Y);
        MyPlot.Title("Зависимость времени от количества тиков");
        MyPlot.XLabel("Тиков на команду");
        MyPlot.YLabel("Время (мс)");
        MyPlot.SavePng("graph.png", 800, 600);

        string Report = "ИЛЛЮСТРАЦИЯ РАБОТЫ ДЛИТЕЛЬНЫХ ОПЕРАЦИЙ\n\n" +
                        "Условия демонстрации:\n" +
                        "- 5 экземпляров TestCommand добавлены в очередь.\n" +
                        "- Каждая команда выполняется 3 раза.\n" +
                        "- После 1000 мс добавляется команда HardStop.\n\n" +
                        "Лог выполнения:\n" +
                        LogText +
                        "\nВывод по демонстрации:\n" +
                        "Поток успешно обработал 5 команд по 3 вызова каждая.\n" +
                        "Команды выполнялись поочередно в порядке Round Robin.\n\n" +
                        "РЕЗУЛЬТАТЫ ЗАМЕРОВ ДЛЯ ГРАФИКА\n" +
                        "Зависимость времени выполнения от количества тиков (5 команд):\n" +
                        "(Каждое значение усреднено по " + RunsCount + " запускам)\n";

        for (int i = 0; i < TicksArray.Length; i++)
        {
            Report = Report + "Тиков: " + TicksArray[i] + ", Среднее время: " + AverageTimes[i].ToString("F2") + " мс\n";
        }

        Report = Report + "\nВывод по графику:\n" +
                        "Время выполнения растёт приблизительно линейно в зависимости\n" +
                        "от количества тиков";

        File.WriteAllText("task19_results.txt", Report);
    }
}
