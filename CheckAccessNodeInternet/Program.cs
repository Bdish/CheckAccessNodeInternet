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
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource(GlobalSetting.LiveTimeProgram);
           // System.Diagnostics.Stopwatch sw = new Stopwatch();
           //   sw.Start();

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

                    //string[] arrayIp = arrayIp1.Take(50000).ToArray();
                    List<PingReply> PingAsync = new List<PingReply>();//массив инициализаций пинга
                    List<Task<PingReply>> pingTasks = null;//массив результатов пинга

                    try
                    {
                     pingTasks = arrayIp.Select(async host => 
                                                            {
                                                                try
                                                                {
                                                                    
                                                                    return await new Ping().SendPingAsync(host, GlobalSetting.TimeOutPing);
                                                                }
                                                                catch (Exception)
                                                                {
                                                                  
                                                                    return null;
                                                                }
                                                            }
                                                                ).ToList();


                        
                        Task.WaitAll(pingTasks.ToArray(), cancelTokenSource.Token);

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Error in the request or the response from the server.");
                    }
 

                    if(pingTasks == null)
                    {
                        Console.WriteLine("Results lost.");
                    }
                   

                    foreach (var ping in pingTasks)
                    {
                        if (ping.Status ==TaskStatus.RanToCompletion)
                        {
                            
                            if (ping.Result != null)
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
            // Console.WriteLine((sw.ElapsedMilliseconds / 1000.0).ToString());

        }

    }

    public static class GlobalSetting
    {
        public static int TimeOutPing { get; set; } = 1000;//пинг 3 секунды

        public static int LiveTimeProgram { get; set; } = 60000;//время жизни проги
    }

}
