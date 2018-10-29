using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckAccessNodeInternetV6
{
    class Program
    {



        static int success = 0;
        static int notSuccess = 0;
        static int statusNotSet = 0;

        static void Main(string[] args)
        {


            CancellationTokenSource cancelTokenSource = new CancellationTokenSource(60000);
          
            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            string path = @"C:\Users\User\Desktop\Выполненые задания для компаний\Ростелеком\CheckAccessNodeInternet - копия\CheckAccessNodeInternet\bin\Debug\netcoreapp2.1\NodeInternet.txt";
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

            string[] arrayIp1 = allIp.Split("\n", StringSplitOptions.RemoveEmptyEntries);//разделяем входную строку с узлами интернет на массив адресов

            string[] arrayIp = new string[50000];

            for (int i= 0;i< 50000; i++)
            {
                arrayIp[i] = arrayIp1[1];
            }

            Task<PingReply>[] pings=new Task<PingReply>[arrayIp.Length];

           

            

            for (int i = 0; i < arrayIp.Length; i++)
            {

                pings[i] =Task.Run(() => { try { return new Ping().SendPingAsync(arrayIp[i], 1000); } catch (Exception) { return null; } }, cancelTokenSource.Token);
                              
            }

            try
            {
                Task.WaitAll(pings);
            }
            catch (Exception)
            {

            }
           


            foreach (var ping in pings)
            {
                if (ping.Status == TaskStatus.RanToCompletion)
                {
                    if (ping.Result.Status == IPStatus.Success)
                    {

                        success++;

                    }
                    else
                    {

                        notSuccess++;

                    }
                }
                else
                {
                    statusNotSet++;
                }

            }


            Console.Clear();
            Console.WriteLine(success.ToString());
            Console.WriteLine(notSuccess.ToString());
            Console.WriteLine(statusNotSet.ToString());
            sw.Stop();
            Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());




            Console.ReadKey();
        }
    }
}
