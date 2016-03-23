using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelComputing.Test
{
    [TestClass]
    public class ParallelTest4
    {
        static object mylock = new object();
        static int sum = 0;
        static int InitLocalState()
        {
            return 0;
        }
        static int RunIteration(int i, ParallelLoopState state, int localsum)
        {
            localsum += i; return localsum;
        }
        static void FinishThread(int localsum)
        {
            lock (mylock) sum += localsum;
        }

        static void TestFor()
        {
            Parallel.For(0, 100, InitLocalState, RunIteration, FinishThread);
            Debug.WriteLine(sum);
        }

        [TestMethod]
        public void ParallelForWithThreadLock()
        {
            TestFor();
        }

        //shorten the first test method using lambda expressions
        [TestMethod]
        public void ParallelForWithThreadLockShorten()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            /*
            object mylock = new object();
            int sum = 0;
            Parallel.For(0, 100, () => 0,
                                 (i, state, localsum) => localsum += i,
                                 (localsum) =>
                                 {
                                     lock (mylock) sum += localsum;
                                 });
            Debug.WriteLine(sum);
            */
            /*
            However, as i mentioned before, you rarely need to write code like this yourself,
            since PLINQ brings us the same speed as Parallel.For, 
            but with much more user - friendliness, letting us do the above accumulation with
            */
            
            int sum = ParallelEnumerable.Range(0, 100).Sum(i => i);
            Debug.WriteLine(sum);
            
            stopWatch.Stop();
            Debug.WriteLine("Total taken time {0}ms", stopWatch.Elapsed.TotalMilliseconds);
        }


    }
}

