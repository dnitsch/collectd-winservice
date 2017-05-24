using System;
using RestSharp;
using System.Threading.Tasks;

namespace collectdSrv
{

    using Model.Request;
    using Model.Response;
    using Properties;

    public class Program
    {


        /// <summary>
        /// entry point into the console app cum service
        /// </summary>
        /// <param name="args"></param>

        static void Main(string[] args)
        {

            ///
            /// load the DataInputObject once
            ///

            DataInputObj dio = SimpleJson.DeserializeObject<DataInputObj>(Resources.ExternalConstants);

            int sleepInterval = (args.Length == 0) ? 60 : Convert.ToInt32(args[0]);

            ///
            /// initialize the constructor with DIO
            ///
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
                 /// quit if not running as service

                } while (!(Environment.UserInteractive));


            }
            catch (Exception e)
            {
                ///proper logging needs to be implemented here
                ///
                Console.WriteLine(e.InnerException);

            }

        }

    }

}




//if (res.OK)
//{

//    Thread.Sleep(sleepInterval);

//}
//else {


//    Environment.Exit(1);

//}

//} while (1 == 1);