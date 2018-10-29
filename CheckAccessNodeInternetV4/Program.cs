using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace CheckAccessNodeInternetV4
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

            PingReply ping;

            for (int i = 0; i < arrayIp.Length; i++)
            {

                try
                {
                   ping = new Ping().SendPingAsync(arrayIp[i], 1000).Result;
                }
                catch (Exception)
                {
                    ping = null;
                }

                if (ping!=null)
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

                Console.Clear();
                Console.WriteLine(success.ToString());
                Console.WriteLine(notSuccess.ToString());
                Console.WriteLine(statusNotSet.ToString());
                Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());
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
