using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CheckAccessNodeInternetV5
{
    class Program
    {
        static void Main(string[] args)
        {


            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();



            //для статистики сбор результатов
            int success = 0;
            int notSuccess = 0;
            int statusNotSet = 0;

            string path = @"C:\Users\User\Desktop\Выполненые задания для компаний\Ростелеком\CheckAccessNodeInternet - копия\CheckAccessNodeInternet\bin\Debug\netcoreapp2.1\NodeInternet.txt";
            string allIp = "";

            try
            {

                allIp = File.ReadAllText(path, Encoding.Default); ;

            }
            catch (Exception)
            {
                Console.WriteLine("Error reading internet nodes from file.");
            }

            if (allIp == "")
            {
                Console.WriteLine("File with internet nodes is empty.");
                return;
            }

            allIp = allIp.Replace("\r", "");//перенос каретки также есть в файле

            string[] arrayIp1 = allIp.Split("\n", StringSplitOptions.RemoveEmptyEntries);//разделяем входную строку с узлами интернет на массив адресов

            string[] arrayIp = new string[1000];

            for (int i = 0; i < 1000; i++)
            {
                arrayIp[i]=arrayIp1[i];
            }


            List<PingReply> PingAsync = new List<PingReply>();//массив инициализаций пинга
            Task<PingReply[]> pingResults = null;//массив результатов пинга

            try
            {
                var pingTasks = arrayIp.Select(host => new Ping().SendPingAsync(host, 1000)).ToList();

                //var pingTasks = arrayIp.Select(host => new Ping().SendPingAsync(host, 5000)).ToList();
                //  Task.WhenAny(pingResults = Task.WhenAll(pingTasks), Task.Delay(60000));
                pingResults = Task.WhenAll(pingTasks);

            }
            catch (Exception)
            {
                Console.WriteLine("Error in the request or the response from the server.");
            }

            // var pingResults =Task.WhenAll(pingTasks); 

            if (pingResults == null)
            {
                Console.WriteLine("Results lost.");
            }
            
            PingAsync = pingResults.Result.ToList();

            foreach (var ping in PingAsync)
            {
                if (ping != null)
                {
                    if (ping.Status == IPStatus.Success)
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







            sw.Stop();
            Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());
        }
        
    }
}
