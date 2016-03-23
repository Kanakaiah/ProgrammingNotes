using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace ParallelComputing.Test
{
    [TestClass]
     public class ParallelTest1
    {
        [TestMethod]
        public void ParallelVersusSequenceTest()
        {
            DateTime startTime = DateTime.Now;
            //Sequential invoking - meaning no parallel computing
            Debug.WriteLine(("Sequential invoking - meaning no parallel computing"));
            F1();
            F2();
            F3();
            DateTime endTime = DateTime.Now;
            Debug.WriteLine((endTime - startTime).Seconds.ToString() + " Milli Seconds");
            Debug.WriteLine(("-----------------------------------"));
            // Parallel invoking
            Debug.WriteLine(("Parallel invoking all the methods"));

            startTime = DateTime.Now;
            Parallel.Invoke(F1, F2, F3);
            endTime = DateTime.Now;
            Debug.WriteLine((endTime - startTime).Seconds.ToString() + " Milli Seconds");
            
        }

        static void F1()
        {
            Thread.Sleep(3000);
            Debug.WriteLine("F1()");
        }
        static void F2()
        {
            Thread.Sleep(3000);
            Debug.WriteLine("F2()");
        }
        static void F3()
        {
            Thread.Sleep(3000);
            Debug.WriteLine("F3()");
        }
    }
}
