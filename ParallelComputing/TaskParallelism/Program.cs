using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskParallelism
{
    class Program
    {
        static Random ran = new Random();


        static void Main(string[] args)
        {
            TaskWaitTest();

        }

        #region TaskWaitTest...
        public static void TaskWaitTest()
        {

            /********* Without a return value ***********/
            //Wait on a single task with no timeout.
            Task taskA = Task.Factory.StartNew(() => Worker(10000)); //10 seconds
            taskA.Wait();
            Console.WriteLine("Task A Finished.");
            //Wait on a single task with a timeout.
            Task taskB = Task.Factory.StartNew(() => Worker(2000000));
            taskB.Wait(2000); //wait for 2 seconds

            if (taskB.IsCompleted)
                Console.WriteLine("Task B Finished");
            else
                Console.WriteLine("Timed out without Task B Finishing");
            Console.ReadLine();

            /********* With a return value ***********/
            //Wait on a single task with no timeout.
            Task<double> taskC = Task.Factory.StartNew(() => Worker1()); //10 seconds
            Console.WriteLine($"Task C Finished. The value is {taskC.Result}");
            Console.ReadLine();
        }


        private static void Worker(int waitTime)
        {
            Thread.Sleep(waitTime);
        }


        private static double Worker1()
        {
            int i = ran.Next(1000000);
            Thread.SpinWait(i);
            return i;
        }
        #endregion
    }
}



