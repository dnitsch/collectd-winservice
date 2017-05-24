using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace collectdSrv.Model.Request
{
   
    public class CollectD
    {

            public Int64 values { get; set; }
            public string[] dstypes { get; set; }
            public string[] dsnames { get; set; }
            public string time { get; set; }
            public int interval { get; set; }
            public string host { get; set; }
            public string plugin { get; set; }
            public string plugin_instance { get; set; }
            public string type { get; set; }
            public string type_instance { get; set; }
    }

    public class CollectorMain
    {
        public List<CollectD> stuff { get; set; }

    }

    public class configSection
    {
        public string name { get; set; }
        public string instance { get; set; }
    }
}
