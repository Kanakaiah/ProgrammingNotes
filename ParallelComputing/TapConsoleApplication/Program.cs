using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TapConsoleApplication.Scheduler;

namespace TapConsoleApplication
{
    class Program
    {
        

        static void Main(string[] args)
        {
            //1
            //GetPrimeCount();
            //2
            //var ElapsedTime1 = Common.With.Benchmark(IterationTest1);
            //Console.WriteLine("Time Elapsed {0}ms", ElapsedTime1);
            //var ElapsedTime2 = Common.With.Benchmark(IterationTest2);
            //Console.WriteLine("Time Elapsed {0}ms", ElapsedTime2);
            //3
            //var ElapsedTime=With.Benchmark(TaskCompletionSourceTest);
            //Console.WriteLine("Time Elapsed {0}ms", ElapsedTime);
            //4
            //var ElapsedTime=With.Benchmark(TaskFactoryTest);
            //Console.WriteLine("Time Elapsed {0}ms", ElapsedTime);
            //5
            TaskSchedulerTest();

        }

        private static void TaskSchedulerTest()
        {
            // Create a scheduler that uses two threads. 
            LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(2);
            List<Task> tasks = new List<Task>();

            // Create a TaskFactory and pass it our custom scheduler. 
            TaskFactory factory = new TaskFactory(lcts);
            CancellationTokenSource cts = new CancellationTokenSource();

            // Use our factory to run a set of tasks. 
            Object lockObj = new Object();
            int outputItem = 0;

            for (int tCtr = 0; tCtr <= 4; tCtr++)
            {
                int iteration = tCtr;
                Task t = factory.StartNew(() => {
                    for (int i = 0; i < 1000; i++)
                    {
                        lock (lockObj)
                        {
                            Console.Write("{0} in task t-{1} on thread {2}   ",
                                          i, iteration, Thread.CurrentThread.ManagedThreadId);
                            outputItem++;
                            if (outputItem % 3 == 0)
                                Console.WriteLine();
                        }
                    }
                }, cts.Token);
                tasks.Add(t);
            }
            // Use it to run a second set of tasks.                       
            for (int tCtr = 0; tCtr <= 4; tCtr++)
            {
                int iteration = tCtr;
                Task t1 = factory.StartNew(() => {
                    for (int outer = 0; outer <= 10; outer++)
                    {
                        for (int i = 0x21; i <= 0x7E; i++)
                        {
                            lock (lockObj)
                            {
                                Console.Write("'{0}' in task t1-{1} on thread {2}   ",
                                              Convert.ToChar(i), iteration, Thread.CurrentThread.ManagedThreadId);
                                outputItem++;
                                if (outputItem % 3 == 0)
                                    Console.WriteLine();
                            }
                        }
                    }
                }, cts.Token);
                tasks.Add(t1);
            }

            // Wait for the tasks to complete before displaying a completion message.
            Task.WaitAll(tasks.ToArray());
            cts.Dispose();
            Console.WriteLine("\n\nSuccessful completion.");

        }

        private static void TaskFactoryTest()
        {
            /***** Without T Results *******/

            //Task[] tasks = new Task[2];
            //String[] files = null;
            //String[] dirs = null;
            //String docsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            //tasks[0] = Task.Factory.StartNew(() => files = Directory.GetFiles(docsDirectory));
            //tasks[1] = Task.Factory.StartNew(() => dirs = Directory.GetDirectories(docsDirectory));

            //Task.Factory.ContinueWhenAll(tasks, completedTasks =>
            //{
            //    Console.WriteLine("{0} contains: ", docsDirectory);
            //    Console.WriteLine("   {0} subdirectories", dirs.Length);
            //    Console.WriteLine("   {0} files", files.Length);
            //}).Wait();

            /***** With T Results *******/
            Task<string[]>[] tasks = new Task<string[]>[2];
            String docsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            tasks[0] = Task<string[]>.Factory.StartNew(() => Directory.GetFiles(docsDirectory));
            tasks[1] = Task<string[]>.Factory.StartNew(() => Directory.GetDirectories(docsDirectory));

            Task.Factory.ContinueWhenAll(tasks, completedTasks => {
                Console.WriteLine("{0} contains: ", docsDirectory);
                Console.WriteLine("   {0} subdirectories", tasks[1].Result.Length);
                Console.WriteLine("   {0} files", tasks[0].Result.Length);
            }).Wait();


        }

        private static void TaskCompletionSourceTest()
        {
            TaskCompletionSource<int> tcs1 = new TaskCompletionSource<int>();
            Task<int> t1 = tcs1.Task;

            // Start a background task that will complete tcs1.Task
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                tcs1.SetResult(15);
            });

            // The attempt to get the result of t1 blocks the current thread until the completion source gets signaled.
            // It should be a wait of ~1000 ms.
            Stopwatch sw = Stopwatch.StartNew();
            int result = t1.Result;
            sw.Stop();

            Console.WriteLine("(ElapsedTime={0}): t1.Result={1} (expected 15) ", sw.ElapsedMilliseconds, result);

            // ------------------------------------------------------------------

            // Alternatively, an exception can be manually set on a TaskCompletionSource.Task
            TaskCompletionSource<int> tcs2 = new TaskCompletionSource<int>();
            Task<int> t2 = tcs2.Task;

            // Start a background Task that will complete tcs2.Task with an exception
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(1000);
                tcs2.SetException(new InvalidOperationException("SIMULATED EXCEPTION"));
            });

            // The attempt to get the result of t2 blocks the current thread until the completion source gets signaled with either a result or an exception.
            // In either case it should be a wait of ~1000 ms.
            sw = Stopwatch.StartNew();
            try
            {
                result = t2.Result;

                Console.WriteLine("t2.Result succeeded. THIS WAS NOT EXPECTED.");
            }
            catch (AggregateException e)
            {
                Console.Write("(ElapsedTime={0}): ", sw.ElapsedMilliseconds);
                Console.WriteLine("The following exceptions have been thrown by t2.Result: (THIS WAS EXPECTED)");
                for (int j = 0; j < e.InnerExceptions.Count; j++)
                {
                    Console.WriteLine("\n-------------------------------------------------\n{0}", e.InnerExceptions[j].ToString());
                }
            }

        }

        //An alternative, and the most common way to start a task in the .NET Framework 4, is to call the static TaskFactory.StartNew or TaskFactory<TResult>.StartNew method
        private static void IterationTest2()
        {
            var t = Task<int>.Factory.StartNew(() => {
                // Just loop.
                int max = 1000000;
                int ctr = 0;
                for (ctr = 0; ctr <= max; ctr++)
                {
                    if (ctr == max / 2 && DateTime.Now.Hour <= 12)
                    {
                        ctr++;
                        break;
                    }
                }
                return ctr;
            });
            Console.WriteLine("Finished {0:N0} iterations.", t.Result);

        }

        //The most common approach, which is available starting with the .NET Framework 4.5, is to call the static Task.
        //Run<TResult>(Func<TResult>) or Task.Run<TResult>(Func<TResult>, CancellationToken) method.
        private static void IterationTest1()
        {
            var t = Task<int>.Run(() => {
                // Just loop.
                int max = 1000000;
                int ctr = 0;
                for (ctr = 0; ctr <= max; ctr++)
                {
                    if (ctr == max / 2 && DateTime.Now.Hour <= 12)
                    {
                        ctr++;
                        break;
                    }
                }
                return ctr;
            });
            Console.WriteLine("Finished {0:N0} iterations.", t.Result);

        }

        private static void GetPrimeCount()
        {
            int min = 1, count = 5;
            Example xmpl = new Example();
            DateTime startTime = DateTime.Now;
            var result = xmpl.GetPrimeCount(min, count);
            Console.WriteLine(result);

            Console.WriteLine("It took {0} milliseconds to finish the sync call, returned {1}", DateTime.Now.Subtract(startTime).Milliseconds, result);


            startTime = DateTime.Now;
            var result1 = xmpl.GetPrimeCountAsync(min, count);
            Console.WriteLine("It took {0} milliseconds to finish the async call, returned {1}", DateTime.Now.Subtract(startTime).Milliseconds, result1.Result);
            Console.WriteLine(result1.Result);

            Console.ReadLine();
        }

        public static void PrintCurrentThreadId(string methodName)
        {
            Console.WriteLine(string.Format("{0} is in thread id:{1}", methodName, Thread.CurrentThread.ManagedThreadId));

        }

        public static async Task<int> GetPrimeCountAsync(int min, int count)
        {
            return await Task.Run<int>(() =>
            {
                Program.PrintCurrentThreadId("GetPrimeCount");
                return ParallelEnumerable.Range(min, count).Count(n =>
                    Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
                    n % i > 0));
            });
        }

      


        class Example
        {

            public int GetPrimeCount(int min, int count)
            {
                Program.PrintCurrentThreadId("GetPrimeCount");
                return ParallelEnumerable.Range(min, count).Count(n =>
                    Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
                    n % i > 0));
            }

            public async Task<int> GetPrimeCountAsync(int min, int count)
            {


                return await Task.Run<int>(() =>
                {
                    Program.PrintCurrentThreadId("GetPrimeCount");
                    return ParallelEnumerable.Range(min, count).Count(n =>
                        Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i =>
                        n % i > 0));
                });
            }




        }
    }
}
