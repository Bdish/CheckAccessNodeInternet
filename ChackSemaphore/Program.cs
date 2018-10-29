using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace ChackSemaphore
{
    
    class Program
    {
        static System.Diagnostics.Stopwatch sw;
        static readonly SemaphoreSlim s = new SemaphoreSlim(100);

        static CancellationTokenSource cancelTokenSource ;
        static CancellationToken token ;

        static void Main(string[] args)
        {
            cancelTokenSource = new CancellationTokenSource(1000);
            token = cancelTokenSource.Token;

            ParallelOptions po = new ParallelOptions();
            po.CancellationToken = cancelTokenSource.Token;



           sw = new Stopwatch();
            sw.Start();

            int[] intArray = new int[100];
            for (int i = 0; i < intArray.Length; i++)
            {
                intArray[i] = i;
            }

            try
            {
                ParallelLoopResult result = Parallel.ForEach<int>(intArray, po, TestPing);
            }
            catch (Exception)
            {
                Console.WriteLine("Canceled");
            }

            
            sw.Stop();
            Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());
            
            }




            static void TestPing(int i)
            {
                
                    try
                    {
                        s.WaitAsync(token);
                        Thread.Sleep(100);
                        Console.WriteLine(i);

                    }
                    finally
                    {
                        s.Release();
                    }
                
            }



            
        }
    
}

