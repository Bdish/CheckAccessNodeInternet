using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckAccessNodeInternetV2
{
    class Program
    {
        static System.Diagnostics.Stopwatch sw;
        static readonly SemaphoreSlim s = new SemaphoreSlim(100);

        static int success = 0;
        static int notSuccess = 0;
        static int statusNotSet = 0;

        static void Main(string[] args)
        {
            sw = new Stopwatch();
            sw.Start();
             string path =@"C:\Users\User\Desktop\Выполненые задания для компаний\Ростелеком\CheckAccessNodeInternet - копия\CheckAccessNodeInternet\bin\Debug\netcoreapp2.1\NodeInternet.txt";
           // string path = @"C:\Users\User\Desktop\Выполненые задания для компаний\Ростелеком\CheckAccessNodeInternet - копия\CheckAccessNodeInternet\bin\Debug\netcoreapp2.1\NodeInternet1.txt";
            string allIp = "";
           
            try
            {

                allIp = File.ReadAllText(path, Encoding.Default); 

            }
            catch (Exception)
            {
                Console.WriteLine("Error reading internet nodes from file.");
            }

            if (allIp == "")
            {
                Console.WriteLine("File with internet nodes is empty.");
               
            }

            allIp = allIp.Replace("\r", "");//перенос каретки также есть в файле

            string[] arrayIp = allIp.Split("\n", StringSplitOptions.RemoveEmptyEntries);//разделяем входную строку с узлами интернет на массив адресов

            ParallelLoopResult result = Parallel.ForEach<string>(arrayIp, TestPing);
            
            

            Console.WriteLine(success.ToString());
            Console.WriteLine(notSuccess.ToString());
            Console.WriteLine(statusNotSet.ToString());
            sw.Stop();
            Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());


        }

        

        static void TestPing(string url)
        {

            try
            {
                s.Wait();
                Console.WriteLine("Count "+s.CurrentCount);
                PingReply test;
                try
                {
                    test = new Ping().SendPingAsync(url, 1000).Result;
                }
                catch (Exception)
                {
                    test = null;
                }
                if (test != null)
                {
                    if (test.Status == IPStatus.Success)
                    {
                        Interlocked.Increment(ref success);
                       // success++;

                    }
                    else
                    {
                        Interlocked.Increment(ref notSuccess);
                       // notSuccess++;

                    }
                }
                else
                {
                    Interlocked.Increment(ref statusNotSet);
                    //statusNotSet++;
                }
            }
            finally
            {
                s.Release();
            }

            Console.Clear();
            Console.WriteLine(success.ToString());
            Console.WriteLine(notSuccess.ToString());
            Console.WriteLine(statusNotSet.ToString());
            
            Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());

        }
    }

   

    public class PingManyInternetNodes
    {
        CancellationTokenSource cancelTokenSource;
        CancellationToken token;
        private static readonly object Locker = new object();

        string[] arrayIp;

        int indexArrayIp;

        public PingManyInternetNodes()
        {
            cancelTokenSource = new CancellationTokenSource(60000);
            token = cancelTokenSource.Token;
        }

        public void NewPing()
        {
            string ip = "";

            lock (Locker)
            {
                if(indexArrayIp< arrayIp.Length)
                {
                    ip = arrayIp[indexArrayIp];
                }
                else
                {
                    Destroy();
                }
            }

            PingReply ping = new Ping().SendPingAsync(ip, 5000).Result;
        }

        public void Destroy()
        {
            
        }

        public void Run()
        {
            int countProc = Environment.ProcessorCount;

            List<Task<PingReply>> arrayTask = new List<Task<PingReply>>(countProc);

            for(int i = 0; i < countProc; i++)
            {
               
            }


        }
    }
}
