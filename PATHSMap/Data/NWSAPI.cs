using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace PATHSMap.Data
{
    public class NWSAPI
    {
        //setting up http client to make api calls
        private HttpClient _client;
        public NWSAPI(HttpClient client)
        {
            _client = client;
        }

        public string GetJsonString(string url)
        {
            var response = _client.GetAsync(url).Result;
            var json = response.Content.ReadAsStringAsync().Result;
            return json;
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
        public static string CalloutOWM (string language, string units, string zip)
        {
            string key = File.ReadAllText("appsettings.json");
            string APIkey = JObject.Parse(key).GetValue("OpenWeatherMapAPIKey").ToString();
            var client = new HttpClient();
            var response = client.GetAsync("https://api.openweathermap.org/data/2.5/weather?zip={zip code},{country code}&appid={API key}").Result;
            var rawjson =  response.Content.ReadAsStringAsync().Result;
            return rawjson;
        }
        
    }
}
