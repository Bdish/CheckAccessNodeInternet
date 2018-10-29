using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CheckAccessNodeInternetV3
{
    class Program
    {
        static int success = 0;
        static int notSuccess = 0;
        static int statusNotSet = 0;

        static void Main(string[] args)
        {
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

            string[] arrayIp = allIp.Split("\n", StringSplitOptions.RemoveEmptyEntries);//разделяем входную строку с узлами интернет на массив адресов

            int mod = 100;

            List<Task<PingReply>> pings = new List<Task<PingReply>>(mod);

            int startIndex = 0;
            
            int endIndex = mod;
            

            ParallelLoopResult result = Parallel.For(startIndex, endIndex,i=> { pings.Add( new Ping().SendPingAsync(arrayIp[i], 500)); } );

            Task.WhenAll(pings);

           /* int countFor = 50000 % 100;
            for (int i = 1; i < countFor; i++)
            {
                 result = Parallel.For(startIndex, endIndex, i => { pings[i]=new Ping().SendPingAsync(arrayIp[i], 5000); });
                Task.WhenAll(pings);
            }*/

            
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


            Console.WriteLine(success.ToString());
            Console.WriteLine(notSuccess.ToString());
            Console.WriteLine(statusNotSet.ToString());
            Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());
            // Console.ReadKey();

            int countFor = 1000/mod;
            for (int ii = 1; ii < countFor; ii++)
            {
                result = Parallel.For(startIndex, endIndex, i => { pings[i] = new Ping().SendPingAsync(arrayIp[ii*mod+i],500); });
                Task.WhenAll(pings);

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


                Console.WriteLine(success.ToString());
                Console.WriteLine(notSuccess.ToString());
                Console.WriteLine(statusNotSet.ToString());
                Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());

            }
            sw.Stop();
            Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());
            Console.ReadKey();
        }


    }
}
