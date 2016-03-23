using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Print
    {
        public static void Line<T>(T t)
        {
            Console.WriteLine(t);
        }

        public static void Word<T>(T t)
        {
            Console.Write(t);
        }
    }
}
