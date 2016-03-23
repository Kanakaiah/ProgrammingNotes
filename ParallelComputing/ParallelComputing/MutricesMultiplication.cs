using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelComputing
{
    public class Matrices
    {
        public static void Multiply(int size, double[,] m1, double[,] m2, double[,] result)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < size; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }
        }


        public static void MultiplyParallel(int size, double[,] m1, double[,] m2, double[,] result)
        {
            Parallel.For(0, size, delegate (int i)
            {
                for (int j = 0; j < size; j++)
                {
                    result[i, j] = 0;
                    for (int k = 0; k < size; k++)
                    {
                        result[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            });
        }

        public static void Print(int size, double[,] result)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {

                    Console.Write(result[i, j]);
                    Console.Write(" ");

                }
                Console.WriteLine();
            }

        }
    }
}
