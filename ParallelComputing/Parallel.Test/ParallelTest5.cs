using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelComputing.Test
{
    [TestClass]
    public class ParallelTest5
    {
        
        
        [TestMethod]
        public void ParallelForEach()
        {
            long elapsedTime = Common.With.Benchmark(action);
            Debug.WriteLine(string.Format("Took {0} ms",elapsedTime.ToString()));
        }

        private void action()
        {
            String[] files = System.IO.Directory.GetFiles(@"C:\Users\Public\Pictures\Sample Pictures", "*.jpg");
            String newDir = @"C:\Users\Public\Pictures\Sample Pictures\Modified";
            System.IO.Directory.CreateDirectory(newDir);



            // Method signature: Parallel.ForEach(IEnumerable<TSource> source, Action<TSource> body)
            // Be sure to add a reference to System.Drawing.dll.
            Parallel.ForEach(files, (currentFile) =>
            {
                // The more computational work you do here, the greater 
                // the speedup compared to a sequential foreach loop.
                String filename = System.IO.Path.GetFileName(currentFile);
                var bitmap = new Bitmap(currentFile);

                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                bitmap.Save(Path.Combine(newDir, filename));

                // Peek behind the scenes to see how work is parallelized.
                // But be aware: Thread contention for the Console slows down parallel loops!!!

                Debug.WriteLine("Processing {0} on thread {1}", filename, Thread.CurrentThread.ManagedThreadId);
                //close lambda expression and method invocation
            });

            // Keep the console window open in debug mode.
            Debug.WriteLine("Processing complete.");
        }
    }
}

