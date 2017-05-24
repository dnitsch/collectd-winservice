using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using collectdSrv.Model.Response;
using collectdSrv.Model.Request;
using RestSharp;
using RestSharp.Serializers;

namespace collectdSrv.Service
{
    public class Caller : IDisposable
        {
        static readonly object _locker = new object();
        private static RestClient instance;
        //public CollectorMain  CollectD;
        public ResponseBody ReponseParsed;
        public static string baseUrl =  ConfigurationManager.AppSettings["loggerRestBaseUrl"].ToString(); 
        protected static RestClient RestClientInstance
            {
                get
                {
                    lock (_locker)
                    {
                        if (instance == null)
                        {
                            instance = new RestClient();
                        }
                        return instance;
                    }
                }
            }


        
        /// <summary>
        /// accepts request object and passes to receiver 
        /// </summary>
        /// <param collectorObj=""></param>
        /// <returns></returns>

        public ResponseBody collectd(List<CollectD> collectorObj )
        {
            ResponseBody returnObj = null;
            CollectorMain mainClctor = new CollectorMain();
                lock (_locker)
                {
              
                    String url = baseUrl;
                    var client = RestClientInstance;
                    client.BaseUrl = new Uri(url);
                    var request = new RestRequest(Method.POST);
                    request.AddHeader("cache-control", "no-cache");
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json; charset=utf-8", request.JsonSerializer.Serialize(collectorObj), ParameterType.RequestBody);

                    IRestResponse<ResponseBody> response = client.Execute<ResponseBody>(request);
                    returnObj = response.Data;

                }

            return returnObj;
        }




        bool disposed = false;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            // Protected implementation of Dispose pattern.
            protected virtual void Dispose(bool disposing)
            {
                if (disposed)
                    return;

                if (disposing)
                {
                    //  Dispose Objects here
                }

                // Free any unmanaged objects here.
                //
                disposed = true;
            }
        }
}
