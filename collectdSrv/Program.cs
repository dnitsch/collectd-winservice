using System;
using System.Configuration;
using System.Threading.Tasks;

namespace collectdSrv
{

    using Model.Request;
    using Model.Response;
    using Configuration;
    using Properties;
    using RestSharp;
    using System.Text;
    using System.Collections.Generic;

    public class Program
    {


        /// <summary>
        /// entry point into the console app cum service
        /// load the app config settings once and use them with each interval
        /// </summary>

        public static void Main(string[] args)
        {
            
            DataInputObj dio = new DataInputObj();
            List<Counter> dioCounter = new List<Counter>();
            PerfCounterConfigurationSection countersConf = ConfigurationManager.GetSection("performanceCounters") as PerfCounterConfigurationSection;

            foreach (CounterConfig single in countersConf.Counters)
            {
                Counter singleCounter = new Counter();

                singleCounter.name = single.Name;
                singleCounter.counterName = single.CounterName.Split(',');
                singleCounter.instanceName = single.InstanceName.Split(',');
                dioCounter.Add(singleCounter);
            }

            dio.counters = dioCounter;

            int sleepInterval = (args.Length == 0) ? 1 : Convert.ToInt32(args[0]);
            Worker.Worker Wrker = new Worker.Worker(dio);
            try {
                do
                {
                    var stopper = Task.Run(async delegate
                        {
                            await Task.Delay(sleepInterval*1000);

                            ResponseBody res = null;

                           res = Wrker.createCollectD(sleepInterval);

                        });
                    stopper.Wait();
                 
                } while (!(Environment.UserInteractive));

            }
            catch (Exception e)
            {
                ///proper logging should be implemented here
                ///
                if (Environment.UserInteractive)
                {
                    Console.WriteLine("Message: " + e.Message + "");
                    Console.WriteLine("InnerException: " + e.InnerException + "");
                    Console.WriteLine("Stack: " + e.StackTrace + "");
                    Console.ReadLine();

                }
            }

        }

    }

}