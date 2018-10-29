using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CheckAccessNodeInternet
{
    class Program
    {
        

        static void Main(string[] args)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource(60000);
            CancellationToken token = cancelTokenSource.Token;
          //  System.Diagnostics.Stopwatch sw = new Stopwatch();
          //  sw.Start();


            CommandLineApplication commandLineApplication =new CommandLineApplication(throwOnUnexpectedArg: false);

            var inFile = commandLineApplication.Option(
            "--inFile|-in",
            "File name with list of internet nodes",
            CommandOptionType.SingleValue);

            var outFile = commandLineApplication.Option(
                "-outFile|-out",
                "File name to save the result of checking the list of Internet nodes.",
                CommandOptionType.SingleValue);

            commandLineApplication.HelpOption("-? | -h | --help");


            commandLineApplication.OnExecute(() =>
            {
                //для статистики сбор результатов
                int success = 0;
                int notSuccess = 0;
                int statusNotSet = 0;

                //указан входной файл с узлами
                if (inFile.HasValue())
                {
                    string allIp="";

                    try { 
                        
                        allIp =File.ReadAllText(inFile.Value(), Encoding.Default); ;

                    }
                        catch (Exception)
                    {
                        Console.WriteLine("Error reading internet nodes from file.");
                    }

                    if (allIp == "")
                    {
                        Console.WriteLine("File with internet nodes is empty.");
                        return 0;
                    }

                    allIp = allIp.Replace("\r", "");//перенос каретки также есть в файле

                    string[] arrayIp = allIp.Split("\n", StringSplitOptions.RemoveEmptyEntries);//разделяем входную строку с узлами интернет на массив адресов

                   /* List<string> arrayIp=new List<string>(50000);

                    for(int i = 0; i < 50000; i++)
                    {
                        arrayIp.Add(arrayIp1[i]);
                    }*/


                    List<PingReply> PingAsync = new List<PingReply>();//массив инициализаций пинга
                    Task<PingReply[]> pingResults=null;//массив результатов пинга

                    try
                    {
                        var pingTasks = arrayIp.Select(host => new Ping().SendPingAsync(host, 3000)).ToList();


                        // pingResults = Task.WhenAll(pingTasks);
                        Task.WhenAny(pingResults = Task.WhenAll(pingTasks), Task.Delay(60000));
                       // Task.WhenAny(pingResults = Task.WhenAll(pingTasks), cancelTokenSource);

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error in the request or the response from the server.");
                    }

                   // var pingResults =Task.WhenAll(pingTasks); 

                    if(pingResults == null)
                    {
                        Console.WriteLine("Results lost.");
                    }
                    PingAsync=pingResults.Result.ToList();

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

                    
                }
                else
                {
                    Console.WriteLine("The file is not specified with Internet nodes.");
                    return 0;
                }

                //указан выходной файл с результатами
                if (outFile.HasValue())
                {
                    try
                    {
                        //формат CVS
                        string result = $"{success};\n{notSuccess};\n{statusNotSet};";

                        if (File.Exists(outFile.Value()))
                        {
                            File.Delete(outFile.Value());
                        }
                        File.Create(outFile.Value()).Close();

                        File.WriteAllText(result, outFile.Value(), Encoding.UTF8);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error saving results.");
                    }
                }
                return 0;
            });

            commandLineApplication.Execute(args);

           // sw.Stop();
          //  Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());
        }
      
    }
    
}
