using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelComputing
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    class ConcurrentCollection
    {
        public static void Run()
        {
            ConcurrentQueue<int> queue = new ConcurrentQueue<int>();
            //Sum of a single thread adding the numbers as we queue them.
            int SingleThreadSum = 0;
            // Populate the queue.
            for (int i = 0; i < 5000; i++)
            {
                SingleThreadSum += i; queue.Enqueue(i);
            }
            //Print the Sum of 0 to 5000.
            Console.WriteLine("Single Thread Sum = {0}", SingleThreadSum);
            //Sum of a multithread adding of the numbers.
            int MultiThreadSum = 0;
            //Create an Action delegate to dequeue items and sum them.
            Action localAction = () =>
            {
                int localSum = 0;
                int localValue;
                while (queue.TryDequeue(out localValue))
                    localSum += localValue;
                Interlocked.Add(ref MultiThreadSum, localSum);
            };
            // Run 3 concurrent Tasks.
            Parallel.Invoke(localAction, localAction, localAction);
            //Print the Sum of 0 to 5000 done by 3 separate threads.
            Console.WriteLine("MultiThreaded  Sum = {0}", MultiThreadSum); Console.ReadLine();
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//namespace TaskFactoryExample
//{
//    class TaskFactoryExample
//    {
//        static TaskFactory TF = new TaskFactory(TaskScheduler.Default);

//        static void Main(string[] args)
//        {
//            List<Task> tasklist = new List<Task>();
//            tasklist.Add(TF.StartNew(() => Worker("Task 1"), CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default));
//            tasklist.Add(TF.StartNew(() => Worker("Task 2"), CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default));
//            tasklist.Add(TF.StartNew(() => Worker("Task 3"), CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default));
//            tasklist.Add(TF.StartNew(() => Worker("Task 4"), CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default));
//            tasklist.Add(TF.StartNew(() => Worker("Task 5"), CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Default));
//            //wait for all tasks to complete.             
//            Task.WaitAll(tasklist.ToArray());
//            //Wait for input before ending program. 
//            Console.ReadLine();
//        }
//        static void Worker(String taskName)
//        {
//            Console.WriteLine("This is Task - {0}", taskName);
//        }
//    }
//}