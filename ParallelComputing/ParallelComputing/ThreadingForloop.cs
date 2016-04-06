using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelComputing
{
    public class ThreadingForloop
    {
        
        public static void StartSequence()
        {
            int lowerBound = 0, upperBound = 1000000;
            for (int i = lowerBound; i < upperBound; i++)
            {
                Console.Write(i);
            }
        }

        public static void StartThread()
        {
            int lowerBound = 0, upperBound = 1000000;
            int numThreads = Environment.ProcessorCount;
            int chunkSize = (upperBound - lowerBound) / numThreads;
            
            var threads = new Thread[numThreads];
            for (int t = 0; t < threads.Length; t++)
            {
                int start = (chunkSize * t) + lowerBound;
                int end = t < threads.Length - 1 ? start + chunkSize : upperBound;
                threads[t] = new Thread(delegate () {
                    for (int i = start; i < end; i++)
                    {
                        Console.Write(i);
                    }
                });
            }

            foreach (Thread t in threads) t.Start(); // fork
            foreach (Thread t in threads) t.Join();  // join

        }

        public static void StartParallel()
        {
            int lowerBound = 0, upperBound = 1000000;
            Parallel.For(lowerBound, upperBound, i => { Console.Write(i); 
            });
        }

    }
}

