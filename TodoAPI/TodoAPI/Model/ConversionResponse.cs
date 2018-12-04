using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoAPI.Model
{
    //class representing values pulled from fixer API
    public class ConversionResponse
    {
        public bool success;

        public int timestamp;

        [JsonProperty("base")]
        public string baseCurrency;

        public string date;

        public Dictionary<string, double> rates;
        

    }
}
