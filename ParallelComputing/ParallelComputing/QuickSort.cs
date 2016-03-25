using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelComputing
{
    public class QuickSort
    {
        public static void Quicksort(IComparable[] elements, int left, int right)
        {
            int i = left, j = right;
            IComparable pivot = elements[(left + right) / 2];

            while (i <= j)
            {
                while (elements[i].CompareTo(pivot) < 0)
                {
                    i++;
                }

                while (elements[j].CompareTo(pivot) > 0)
                {
                    j--;
                }

                if (i <= j)
                {
                    // Swap
                    IComparable tmp = elements[i];
                    elements[i] = elements[j];
                    elements[j] = tmp;

                    i++;
                    j--;
                }
            }

            // Recursive calls
            if (left < j)
            {
                Quicksort(elements, left, j);
            }

            if (i < right)
            {
                Quicksort(elements, i, right);
            }
        }

        static int THRESHOLD = 100;

        static void QuickSortParallel<T>(T[] data, int lower, int upper)
        {
            if((upper- lower)<THRESHOLD)
            {
                Array.Sort(data, index: lower, length: upper - lower);
            }
            else { 
                int pivotPos = Partition(data, lower, upper);
                Parallel.Invoke(
                  () => QuickSortParallel(data, lower, pivotPos),
                  () => QuickSortParallel(data, pivotPos, upper));
            }
        }

        private static int Partition<T>(T[] data, int lower, int upper)
        {
            return 1000;
        }
    }

}
