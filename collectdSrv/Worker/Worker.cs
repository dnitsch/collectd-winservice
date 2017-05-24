using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RestSharp;
using System.Configuration;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace collectdSrv.Worker
{
    using Service;
    using Model.Request;
    using Model.Response;
    using Configuration;
    public class Worker
    {

        Caller invoker = new Caller();

        public string categoryName = String.Empty;

        public string machineName = Environment.MachineName;

        public string[] instances { get; set; }

        DataInputObj _dio { get; set; }

        //public string[] categories { get; set; }

        PerformanceCounterCategory pcc; // = new PerformanceCounterCategory();
        PerformanceCounter[] counters; /// = new PerformanceCounter();

        /// <summary>
        /// .ctor creates an instance of the input categories and instances for the looping below
        /// </summary>
        public Worker(DataInputObj dio) {

            _dio = dio;

        }


        /// <summary>
        /// runs the perfcounter .net caller
        /// </summary>
        /// <param name="sleeper"></param>
        /// <returns></returns>
        public ResponseBody createCollectD (Int32 sleeper) {
            ///
            //TODO
            /// turn into dynamic source
            /// Deprecated using app.config and moved to resources 
            // PerfCounterConfigurationSection
            //var confSection = ConfigurationManager.GetSection("externalConstants");  //perf_counters

            ResponseBody res = new ResponseBody();
            List<CollectD> mainCounter = new List<CollectD>();

            string[] dstype = { "value" };
            string[] dsname = { "gauge" };

            try
            {

                foreach( var xyz in _dio.counters)
                {
                    //int z;
                    pcc = new PerformanceCounterCategory(xyz.name, machineName);

                    instances = pcc.GetInstanceNames();

                    //
                    foreach( var instance in instances)
                    {


                  
                    counters = pcc.GetCounters(instance);

                    
                    // Display a numbered list of the counter names.
                    int objX;
                    for (objX = 0; objX < counters.Length; objX++)
                    {
                            ///filter the list of counters to ones we want
                            /// based on properties input and parsing into a Model.Request sharedinput
                        int position = Array.IndexOf(xyz.instance, counters[objX].CounterName);

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
                            //Console.WriteLine(String.Format("This counter {3} exist in instance {1} of  category {0}  on {2}",
                            //           counters[objX].CategoryName, counters[objX].InstanceName, machineName, counters[objX].CounterName));
                        }
                    }
                // end instance loop
                    }
                }


                res = invoker.collectd(mainCounter);

                return res;
            }
            catch (Exception e)
            {
                res.Message = "error";
                res.OK = false;
                res.ResponseData = e.Message;

                return res;
            }
            finally {

                mainCounter = null;
              

            }


            }

        /// <summary>
        /// helper method 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string categoryConverter(string input) {
            string output = String.Empty;

            switch (input.ToLower()) {
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

                default:
                    output = "unassigned";
                    break;
            }
            return output;


        }

        }

}


/// <summary>
/// 
/// </summary>
//// public ResponseBody collectWorker(string message, string innerException, string stackTrace)
//public void collectWorker()
//{





//// Create the appropriate PerformanceCounterCategory object.
//// foreach key in AppConfig

//pcc = new PerformanceCounterCategory(categoryName, machineName);



// instances = pcc.GetInstanceNames();

// Get the counters for this instance or a single instance 
// of the selected category.


//int objY;


//for (objY = 0; objY < instances.Length; objY++)
//{
//    instanceName = instances[objY];

//    counters = pcc.GetCounters(instanceName);

//    // Display a numbered list of the counter names.
//    int objX;
//    for (objX = 0; objX < counters.Length; objX++)
//    {
//        //Console.WriteLine("{0,4} - {1}", objX + 1, counters[objX].CounterName);
//        Console.WriteLine(String.Format("This counter {3} exist in instance {1} of  category {0}  on {2}",
//                    categoryName, instanceName, machineName, counters[objX].CounterName));
//    }
//}






//res = invoker.collectd(mainCounter);

//if (res.OK == "false")
//{
//    return res;

//}

//return res;

//}
//catch (Exception ex)
//{
//    throw ex;
//}
//finally {
//    mainCounter = null;
//    singleMetric = null;
//    res = null;
//}


//        }
//}

//    }
//string[] perf_name = { "Web Service", "PhysicalDisk", "Processor" };
//string[] instanceType = { "Bytes Received/sec" , "Bytes Sent/sec","Connection Attempts/sec","Current Connections","Current Blocked Async I/O Requests","Current Connections",
//                "Total Options Requests","Total Put Requests","Total Post Requests","Total Get Requests","Total Delete Requests",
//                "Current Disk Queue Length","% Disk Time","Avg. Disk Queue Length","% Disk Read Time","Avg. Disk Read Queue","% Disk Write Time","Avg. Disk Write Queue",
//                "% Processor Time","% User Time","% Privileged Time","Interrupts/sec","% DPC Time","% Interrupt Time","% Idle Time"
//                //,"% C1 Time"
//            };


