using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using task17;
using ScottPlot;

class Program
{
    private class WorkCommand : ICommand
    {
        public int TicksRemaining;

        public WorkCommand(int Ticks)
        {
            TicksRemaining = Ticks;
        }

        public bool Execute()
        {
            //Работа, чтобы занять процессор на один тик
            int Sum = 0;
            for (int I = 0; I < 10000; I++)
            {
                Sum = Sum + I;
            }
            TicksRemaining--;
            return TicksRemaining == 0;
        }
    }

    static void Main()
    {
        int[] Counts = new int[] { 100, 500, 1000, 5000, 10000 };
        List<double> Times = new List<double>();
        int TicksPerCommand = 5;

        foreach (int Count in Counts)
        {
            RoundRobinScheduler Scheduler = new RoundRobinScheduler();
            //Квант ставим 10, просто как рабочее значение
            ServerThread Server = new ServerThread(null, Scheduler, 10);

            for (int I = 0; I < Count; I++)
            {
                Server.Enqueue(new WorkCommand(TicksPerCommand));
            }

            Stopwatch Timer = Stopwatch.StartNew();

            //Ждём, пока планировщик не опустеет
            while (Scheduler.HasCommand())
            {
                System.Threading.Thread.Sleep(1);
            }
            System.Threading.Thread.Sleep(50);

            Timer.Stop();
            Times.Add(Timer.Elapsed.TotalMilliseconds);

            Server.Stop();
            System.Threading.Thread.Sleep(50);
        }
        Plot MyPlot = new Plot();
        double[] X = new double[Counts.Length];
        double[] Y = new double[Counts.Length];

        for (int I = 0; I < Counts.Length; I++)
        {
            X[I] = Counts[I];
            Y[I] = Times[I];
        }

        MyPlot.Add.Scatter(X, Y);
        MyPlot.Title("Зависимость времени от количества команд");
        MyPlot.XLabel("Количество команд");
        MyPlot.YLabel("Время (мс)");
        MyPlot.SavePng("graph.png", 800, 600);
        string Report = "Результаты замеров:\n";
        for (int I = 0; I < Counts.Length; I++)
        {
            Report = Report + "Команд: " + Counts[I] + ", Время: " + Times[I].ToString("F2") + " мс\n";
        }
        Report = Report + "\nВывод: время выполнения растет линейно в зависимости от количества команд в очереди, так как планировщик обрабатывает их последовательно, выделяя каждой команде по одному тику.";

        File.WriteAllText("task18_results.txt", Report);
    }
}
