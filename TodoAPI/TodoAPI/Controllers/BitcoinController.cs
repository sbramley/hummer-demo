using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TodoAPI.HttpRequest;
using TodoAPI.Model;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BitcoinController : ControllerBase
    {
        private readonly ILogger _logger;
        private IHttpGet _httpGet;

        //constructor with dependency injected logger and httpgetter
        public BitcoinController(ILogger<BitcoinController> logger, IHttpGet httpGet)
        {
            _logger = logger;
            _httpGet = httpGet;
        }

        /**
        * get Method that passes parameters onto the bitcoin API and the fixer API
        * Takes an acccess key as a required parameter, and a base, and symbols array as optional parameters
        * returns a  json object in the form provided by the fixer API
        **/ 
        public async Task<ActionResult<string>> Get(string access_key, [FromQuery(Name = "base")] string base_currency = "", string[] symbols = null)
        {

            //create objects for deserialization
            Dictionary<string, BitCoinValues> BitCoinInfo = new Dictionary<string, BitCoinValues>();
            ConversionResponse conversionResponse = new ConversionResponse();
            Dictionary<string, BitcoinRate> rates = new Dictionary<string, BitcoinRate>();

            //wrap each of our api calls in a try/catch in case something goes wrong
            try
            {
                //call each url asynchronously
                Task<string> bitCoinResponsesTask = _httpGet.GetFromURLAsync("https://blockchain.info/ticker");
                string formattedURL = String.Format("http://data.fixer.io/api/latest?access_key={0}&base={1}&symbols={2}", access_key, base_currency, string.Join(",", symbols));
                Task<string> currencyResponsesTask = _httpGet.GetFromURLAsync(formattedURL);

                //await the responses
                string bitCoinResponses = await bitCoinResponsesTask;
                string currencyResponses = await currencyResponsesTask;

                //convert responses to json
                BitCoinInfo = JsonConvert.DeserializeObject<Dictionary<string, BitCoinValues>>(bitCoinResponses);
                conversionResponse = JsonConvert.DeserializeObject<ConversionResponse>(currencyResponses);

                //if something went wrong with fixer, it is assumed to be correct response, as the input for this API endpoint reflects input for fixer
                if (conversionResponse.success == false)
                {
                    _logger.LogWarning(String.Format("Currency conversion came back with code not equal to 200: {0}", conversionResponse));
                    return currencyResponses;
                }
            }

            //in case something went really wrong, log it and return a 500 error
            catch (Exception e)
            {
                this.HttpContext.Response.StatusCode = 500;
                _logger.LogError(e.Message);
                return "Server side error";
            }

            //match countries returned by fixer with those returned by bitcoin
            //NOTE, the bitcoin endpoint seemed to return a fixed list here. I assumed the fixer list to be correct, as that was specified by the user,
            //if the bitcoin value did not exist, then I returned a default value
            foreach(string country in conversionResponse.rates.Keys)
            {
                BitcoinRate bitcoinRate = new BitcoinRate();
                bitcoinRate.latest = conversionResponse.rates[country];
                if (BitCoinInfo.Keys.Contains(country))
                    bitcoinRate.bitcoinLatest = BitCoinInfo[country].last;
                else
                    bitcoinRate.bitcoinLatest = -1;

                rates.Add(country, bitcoinRate);
            }

            //build response object and return
            ResponseJson json = new ResponseJson(conversionResponse, rates);

            return JsonConvert.SerializeObject(json, Formatting.Indented);
        }
    }
}
