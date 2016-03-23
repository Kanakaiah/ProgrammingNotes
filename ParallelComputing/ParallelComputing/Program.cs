using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Threading;
using System.Diagnostics;

namespace ParallelComputing
{
    class Program
    {
        static void Main(string[] args)
        {
            //1 
            //ParallelInvoke();
            //2
            //ParallelFor();
            //3
            long ElapesedTime1 = Common.With.Benchmark(ThreadingForloop.StartSequence);
            long ElapesedTime2 = Common.With.Benchmark(ThreadingForloop.StartThread);
            long ElapesedTime3 = Common.With.Benchmark(ThreadingForloop.StartParallel);

            Console.WriteLine();
            Console.WriteLine("Sequence--Time Taken {0}ms", ElapesedTime1);
            Console.WriteLine("Threading--Time Taken {0}ms", ElapesedTime2);
            Console.WriteLine("Parallel--Time Taken {0}ms", ElapesedTime3);

            /* Output
            Sequence--Time Taken 8050ms
            Threading--Time Taken 9664ms
            Parallel--Time Taken 7181ms
            */

        }


        private static void ParallelInvoke()
        {
            DateTime startTime = DateTime.Now;
            //Sequential invoking - meaning no parallel computing
            Print.Line(("Sequential invoking - meaning no parallel computing"));
            F1();
            F2();
            F3();
            DateTime endTime = DateTime.Now;
            Print.Line((endTime - startTime).Seconds.ToString() + " Milli Seconds");
            Print.Line(("-----------------------------------"));
            // Parallel invoking
            Print.Line(("Parallel invoking all the methods"));

            startTime = DateTime.Now;
            Parallel.Invoke(F1, F2, F3);
            endTime = DateTime.Now;
            Print.Line((endTime - startTime).Seconds.ToString() + " Milli Seconds");

            Console.Read();
        }

        static void F1()
        {
            Thread.Sleep(3000);
            Print.Line<string>("F1()");
        }
        static void F2()
        {
            Thread.Sleep(3000);
            Print.Line<string>("F2()");
        }
        static void F3()
        {
            Thread.Sleep(3000);
            Print.Line<string>("F3()");
        }

        static void ParallelFor()
        {

            double[,] m1 = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
            double[,] m2 = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
            double[,] result = new double[3, 3];
            int size = 3;

            var stopwatch = Stopwatch.StartNew();
            Matrices.Multiply(size, m1, m2, result);
            stopwatch.Stop();
            Console.WriteLine("Elapsed time {0} ms", stopwatch.ElapsedMilliseconds);
            Matrices.Print(size, result);

            stopwatch = Stopwatch.StartNew();
            Matrices.MultiplyParallel(size, m1, m2, result);
            stopwatch.Stop();
            Console.WriteLine("Elapsed time {0} ms", stopwatch.ElapsedMilliseconds);
            Matrices.Print(size, result);
            ;
        }
        
    }
}
