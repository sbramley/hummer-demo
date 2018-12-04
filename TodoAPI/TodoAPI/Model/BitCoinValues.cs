using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoAPI.Model
{

    //class representing the values pulled from the bitcoin API
    public class BitCoinValues
    {
        public double last;
        public double buy;
        public double sell;
        public string symbol;
        [JsonProperty("15m")]
        public double m15;
    }
}
