using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckAccessNodeInternetV7
{
    class Program
    {
        static List<Task<PingReply>> listPings;
        static int success = 0;
        static int notSuccess = 0;
        static int statusNotSet = 0;

        // данная коллекция будет содержать имена рабочих станций
        private static List<string> hosts = new List<string>();

        public static void Main()
        {
            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            // В переменную hosts записываем все рабочие станции из файла
            hosts = getComputersListFromTxtFile(@"C:\Users\User\Desktop\Выполненые задания для компаний\Ростелеком\CheckAccessNodeInternet - копия\CheckAccessNodeInternet\bin\Debug\netcoreapp2.1\NodeInternet.txt");
            // Создаём Action типизированный string, данный Action будет запускать функцию Pinger
            Action<string> asyn = new Action<string>(Pinger);
            // Для каждой рабочей станции запускаем Pinger'а
            
            hosts.Take(50000).ToList().ForEach(e =>
            {
                asyn.Invoke(e);
            });
            Task.WaitAll(listPings.ToArray());

            Console.Clear();
            Console.WriteLine(success.ToString());
            Console.WriteLine(notSuccess.ToString());
            Console.WriteLine(statusNotSet.ToString());
            sw.Stop();
            Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());
            Console.ReadKey();

        }

        // Функция получения списка рабочих станций из файла
        private static List<string> getComputersListFromTxtFile(string pathToFile)
        {
            List<string> computersList = new List<string>();
            using (StreamReader sr = new StreamReader(pathToFile, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    computersList.Add(line);
                }
            }
            return computersList;
        }

        // Функция записи проблемных рабочих станций в файл
        private static void writeProblemComputersToFile(string fullPathToFile, List<string> problemComputersList)
        {
            using (StreamWriter sw = new StreamWriter(fullPathToFile, true, Encoding.Default))
            {
                problemComputersList.ForEach(e => { sw.WriteLine(e); });
            }
        }

        // Проверяем доступна ли папка admin$
        private static bool checkAdminShare(string computerName)
        {
            if (Directory.Exists("\\\\" + computerName + "\\admin$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Наш основной класс, который будет отправлять команду ping
        async private static void Pinger(string hostAdress)
        {
            // Создаём экземпляр класса Ping
            Ping png = new Ping();
            try
            {
                // Пингуем рабочую станцию hostAdress
                Task<PingReply> pr = png.SendPingAsync(hostAdress,10000);
                listPings.Add(pr);


              /*  if (pr.Status == IPStatus.Success)
                {
                    Interlocked.Increment(ref success);
                   

                }
                else
                {
                    Interlocked.Increment(ref notSuccess);
                   
                }*/

             
            }
            catch
            {
             
              //  Interlocked.Increment(ref statusNotSet);

            }

           
        }
    }
}
