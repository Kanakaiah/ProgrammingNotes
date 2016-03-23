using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelComputing.Test
{
    [TestClass]
    public class ParallelTest2
    {
        static CancellationTokenSource tok = new CancellationTokenSource();

        [TestMethod]
        public void CancellationTokenTest()
        {

            ParallelOptions op = new ParallelOptions();
            op.CancellationToken = tok.Token;
            try
            {
                Parallel.Invoke(op, F1, F2, F3);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Cancelled!");
            }

        }

        static void F1()
        {
            //DateTime startTime = DateTime.Now;
            tok.Cancel();
            Thread.Sleep(3000);
            Debug.WriteLine("F1()");
            //Debug.WriteLine((DateTime.Now - startTime).Milliseconds);
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
/*
Notes:
- We create a CancellationTokenSource, and pass its Token property to Invoke through a ParallelOptions instance.
- Now if the token is set to cancel via CancellationTokenSource.Cancel(), a OperationCancelledException is 
  thrown, but this one is thrown immediately from Invoke, prohibiting the execution of any more delegates in
  the array (but delegates which are being executed in the moment that Cancel() is called are completed and 
  not interrupted).
*/
