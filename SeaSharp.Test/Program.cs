using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaSharp.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = new Stopwatch();
            watch.Start();


            watch.Stop();
            Debug.WriteLine("时间是：" + watch.ElapsedMilliseconds + "毫秒");
        }
    }
}
