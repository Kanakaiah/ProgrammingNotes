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
    public class ParallelTest3
    {

        [TestMethod]
        public void ParallelForTest()
        {
            Debug.WriteLine("----------Parallel start------------");
            DateTime startTime = DateTime.Now;
            Parallel.For(0, 10000, i => Debug.Write(i.ToString() + ","));
            Debug.WriteLine("");
            Debug.WriteLine("----------Parallel end------------");
            Debug.WriteLine((DateTime.Now - startTime).Milliseconds);

            Debug.WriteLine("----------Sequence start--------------");
            startTime = DateTime.Now;
            for (int i = 0; i < 10000; i++)
            {
                Debug.Write(i.ToString()+",");
            }
            Debug.WriteLine("");
            Debug.WriteLine("----------Sequence start--------------");
            Debug.WriteLine((DateTime.Now - startTime).Milliseconds);
        }

        [TestMethod]
        public void ParallelForWithParameterTest()
        {
            //Parallel.For(0, 100, (i, state) => {
            //    Debug.WriteLine(i); if (i == 50) state.Stop();
            //});
            /*
            As soon as the iteration in which i equals 50 is executed, the For loop is stopped.
            It may only continue iterations which it already has begun to execute. 
            There is no way to predict which iterations will be executed 
            (unless perhaps you write your own scheduler that follows a predictable pattern).
            */

            //or break method
            Parallel.For(0, 100, (i, state) => {
                Debug.WriteLine(i); if (i == 50) state.Break();
            });
            /*
            in this case, the For loop will only stop scheduling more iterations when it runs the 50th iteration, 
            and not stop execution immediately.
            This means that all the loop iterations where i is between 0 and 50 will run. 
            Additional iterations may be run, if they were already started when Break was called.
            
            If you use the overload with the ParallelLoopState, For will return a ParallelLoopResult, 
            which lets you query if the For loop was completed or aborted (with the IsCompleted property) 
            and which was the lowest index that called Break 
            (with the LowestBreakIteration property, which is a nullable int, and will be null if the loop wasn't aborted).
            The ParallelLoopState also has a property ShouldExitCurrentIteration. 
            You can query this property, and break early in an iteration: it will be true if some thread called Break or Stop, 
            Cancel was called on a cancellation token, or an exception occured in another iteration.
            */

        }
    }
}

/*
Notes:
Parallel.For comes with a handy number of overloads to let you customize its behavior.
 - The first overload accepts a ParallelOptions object as the third argument. This works like it did for Invoke: you can supply a custom scheduler
 - Set a maximum number of concurrent threads to use, 
 - and assign a cancel token.
 The last however is not really needed, since another overload provides us an easier way to break out of the loop than using a cancellation token.
 Instead of an Action<T>, you can pass an Action<T, ParallelLoopState>, in which you can use the ParallelLoopState to break from the loop.
 */


/* Outputs generated on a 8 core machine
0	0	0	0
12	12	12	12
13	13	24	13
14	24	25	14
15	25	26	24
16	26	27	36
17	27	28	37
18	28	29	38
48	29	30	72
60	48	31	60
24	36	36	48
36	60	48	84
72	84	60	96
84	72	96	73
96	96	13	74
1	97	72	75
2	98	1	1
3	1	84	15
4	14	2	25
5	30	32	39
6	49	33	61
19	61	34	49
49	62	37	85
37	63	49	97
25	85	61	2
85	73	14	3
73	37	73	76
61	38	97	77
97	99	85	16
98	3	86	17
99	31	38	26
27	2	3	40
7	15	4	62
20	50	35	50
38	64	62	41
50	86	50	86
26	74	15	98
86	4	43	63
62	39	74	18
74	32	5	27
8	16	98	78
28	11	87	4
39		63	
21		39	
*/