using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace collectdSrv.Model.Request
{

    public class Counter
    {
        public string name { get; set; }
        public string[] counterName { get; set; }
        public string[] instanceName { get; set; }
    }


    public class DataInputObj
    {
        public List<Counter> counters { get; set; }
    }


}
