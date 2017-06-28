using System;
using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using RestSharp;
//using System.Configuration;
//using System.Threading.Tasks;
using System.Diagnostics;
//using System.Reflection;

namespace collectdSrv.Worker
{
    using Service;
    using Model.Request;
    using Model.Response;
    using Configuration;
    using System.Text.RegularExpressions;

    public class Worker
    {

        Caller invoker = new Caller();

        public string categoryName = String.Empty;

        public string machineName = Environment.MachineName;

        public string[] instances { get; set; }

        DataInputObj _dio { get; set; }
        List<Counter> _dioCounter { get; set; }
        //public string[] categories { get; set; }

        PerformanceCounterCategory pcc; // = new PerformanceCounterCategory();
        PerformanceCounter[] counters; /// = new PerformanceCounter();

        /// <summary>
        /// .ctor creates an instance of the input categories and instances for the looping below
        /// </summary>
        public Worker(DataInputObj dio)
        {
            _dio = dio;
        }

        public Worker(List<Counter> dioCounter)
        {
            _dioCounter = dioCounter;
        }

        /// <summary>
        /// runs the perfcounter .net caller
        /// </summary>
        /// <param name="sleeper"></param>
        /// <returns></returns>
        public ResponseBody createCollectD(Int32 sleeper)
        {
            ///
            //TODO
            /// turn into dynamic source
            /// Deprecated using app.config and moved to resources 

            ResponseBody res = new ResponseBody();
            List<CollectD> mainCounter = new List<CollectD>();

            string[] dstype = { "value" };
            string[] dsname = { "gauge" };

            try
            {

                foreach (var xyz in _dio.counters)
                {
                    pcc = new PerformanceCounterCategory(xyz.name, machineName); //

                    instances = pcc.GetInstanceNames();

                    foreach (var instance in instances)
                    {

                        ///
                        /// if instanceName array exists
                        /// we need to filter through 
                        /// else we contintue only based on countername filters
                        /// 

                        if (xyz.instanceName.Length > 0)
                        {

                            foreach (string spqr in xyz.instanceName)
                            {
                                if (instance.Contains(spqr))
                                {
                                    counters = pcc.GetCounters(instance);
                                    foreach (var counter in counters)
                                    {

                                        if (instance.Contains(spqr))
                                        {
                                            int position = Array.IndexOf(xyz.counterName, counter.CounterName);

                                            if (position > -1)
                                            {
                                                //Console.WriteLine(counter.CategoryName);
                                                CollectD singleMetric = new CollectD();
                                                singleMetric.host = machineName;
                                                singleMetric.dstypes = dstype;
                                                singleMetric.dsnames = dsname;
                                                singleMetric.interval = sleeper;
                                                singleMetric.plugin = categoryConverter(counter.CategoryName);
                                                singleMetric.plugin_instance = counter.InstanceName;
                                                singleMetric.time = 1;
                                                singleMetric.type_instance = Convert.ToString(counter.CounterName);
                                                singleMetric.type = "gauge";
                                                singleMetric.values = Convert.ToInt64(counter.RawValue);
                                                mainCounter.Add(singleMetric);

                                                if (counter.CategoryName == "Process")
                                                {
                                                    Console.Write("" + counter.InstanceName + "\n");
                                                }
                                            }
                                        }

                                    }
                                }

                            }
                        }
                        else
                        {

                            counters = pcc.GetCounters(instance);
                            // Display a numbered list of the counter names.
                            int objX;
                            for (objX = 0; objX < counters.Length; objX++)
                            {
                                ///filter the list of counters to ones we want
                                /// based on properties input and parsing into a Model.Request sharedinput
                                int position = Array.IndexOf(xyz.counterName, counters[objX].CounterName);

                                if (position > -1)
                                {
                                    CollectD singleMetric = new CollectD();
                                    singleMetric.host = machineName;
                                    singleMetric.dstypes = dstype;
                                    singleMetric.dsnames = dsname;
                                    singleMetric.interval = sleeper;
                                    singleMetric.plugin = categoryConverter(counters[objX].CategoryName);
                                    singleMetric.plugin_instance = counters[objX].InstanceName;
                                    singleMetric.time = 1;
                                    singleMetric.type_instance = Convert.ToString(counters[objX].CounterName);
                                    singleMetric.type = "gauge";
                                    singleMetric.values = Convert.ToInt64(counters[objX].RawValue);
                                    mainCounter.Add(singleMetric);
                                }
                            }
                        }
                        // end instance loop
                    }
                }
                if (Environment.UserInteractive)
                {
                    Console.WriteLine("total rows: " + mainCounter.Count + "");

                }
                res = invoker.collectd(mainCounter);

                return res;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                mainCounter = null;
            }
        }

        /// <summary>
        /// helper method to normalise data across OS types/versions
        /// collectd on linux provides: disk, cpu, memory, aggregation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string categoryConverter(string input)
        {
            string output = String.Empty;

            switch (input.ToLower())
            {
                case "physicaldisk":
                    output = "disk";
                    break;
                case "physical disk":
                    output = "disk";
                    break;
                case "memory":
                    output = "memory";
                    break;
                case "processor":
                    output = "cpu";
                    break;
                case "web service":
                    output = "iis";
                    break;
                case "network interface":
                    output = "network";
                    break;
                case "process":
                    output = "process";
                    break;
                //case ".NET CLR Memory":
                //    output = "process";
                //    break;
                //case ".NET CLR LocksAndThreads":
                //    output = "process";
                //    break;
                case "W3SVC_W3WP":
                    output = "iis";
                    break;
                case "HTTP Service":
                    output = "iis";
                    break;
                case "HTTP Service Request Queues":
                    output = "iis";
                    break;
                default:
                    output = "unassigned";
                    break;
            }
            return output;


        }

        /// <summary>
        /// ISODate creates a TSDB friendly timestamp 
        /// </summary>
        /// <returns>ISO date string 
        /// @isoDateNow (String)
        /// </returns>
        public string ISODateNow()
        {
            string isoDateNow = DateTime.UtcNow.ToString("O");
            return isoDateNow;
        }

    }

}