using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TodoAPI.Model
{

    //object representing values returned to customer
    public class ResponseJson
    {
        public ResponseJson(ConversionResponse conversionResponse, Dictionary<string, BitcoinRate> rates)
        {
            success = conversionResponse.success;
            timestamp = conversionResponse.timestamp;
            date = conversionResponse.date;
            baseCurrency = conversionResponse.baseCurrency;
            this.rates = rates;
        }

        public bool success;

        public long timestamp;
        
        [JsonProperty("base")]
        public string baseCurrency;

        public string date;

        public Dictionary<string, BitcoinRate> rates;

    }
}
