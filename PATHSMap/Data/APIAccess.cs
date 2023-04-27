using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;

namespace PATHSMap.Data
{
    public class APIAccess
    {
        //setting up http client to make api calls
        private HttpClient _client;
        public APIAccess(HttpClient client)
        {
            _client = client;
        }



        public static string CalloutNWS (string url)
        {
            var client = new HttpClient();
            var productValue = new ProductInfoHeaderValue("PATHSMap", "Alpha");
            client.DefaultRequestHeaders.UserAgent.Add(productValue);
            var response = client.GetAsync(url).Result;
            var rawjson =  response.Content.ReadAsStringAsync().Result;
            return rawjson;
        }

        
    }
}
