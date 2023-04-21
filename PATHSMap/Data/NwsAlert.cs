using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PATHSMap.Data
{
    public class Geocode
    {
        [JsonProperty("SAME")]
        public List<string> SAME { get; set; }

        [JsonProperty("UGC")]
        public List<string> UGC { get; set; }
    }
    public class Parameters
    {
        [JsonProperty("AWIPSidentifier")]
        public List<string> AWIPSidentifier { get; set; }

        [JsonProperty("WMOidentifier")]
        public List<string> WMOidentifier { get; set; }

        [JsonProperty("NWSheadline")]
        public List<string> NWSheadline { get; set; }

        [JsonProperty("eventMotionDescription")]
        public List<string> eventMotionDescription { get; set; }

        [JsonProperty("maxWindGust")]
        public List<string> maxWindGust { get; set; }

        [JsonProperty("maxHailSize")]
        public List<string> maxHailSize { get; set; }

        [JsonProperty("BLOCKCHANNEL")]
        public List<string> BLOCKCHANNEL { get; set; }

        [JsonProperty("EAS-ORG")]
        public List<string> EASORG { get; set; }

        [JsonProperty("tornadoDetection")]
        public List<string> tornadoDetection { get; set; }

        [JsonProperty("VTEC")]
        public List<string> VTEC { get; set; }

        [JsonProperty("eventEndingTime")]
        public List<DateTime> eventEndingTime { get; set; }

        [JsonProperty("expiredReferences")]
        public List<string> expiredReferences { get; set; }
    }
    public class Reference
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("identifier")]
        public string identifier { get; set; }

        [JsonProperty("sender")]
        public string sender { get; set; }

        [JsonProperty("sent")]
        public DateTime sent { get; set; }
    }
    public class Geometry
    {
        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("coordinates")]
        public List<List<List<double>>> coordinates { get; set; }
    }
    public class Feature
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("geometry")]
        public Geometry geometry { get; set; }

        [JsonProperty("properties")]
        public Properties properties { get; set; }
    }
    public class Properties
    {
        [JsonProperty("@id")]
        public string id { get; set; }

        [JsonProperty("@type")]
        public string type { get; set; }

        [JsonProperty("areaDesc")]
        public string areaDesc { get; set; }

        [JsonProperty("geocode")]
        public Geocode geocode { get; set; }

        [JsonProperty("affectedZones")]
        public List<string> affectedZones { get; set; }

        [JsonProperty("references")]
        public List<Reference> references { get; set; }

        [JsonProperty("sent")]
        public DateTime sent { get; set; }

        [JsonProperty("effective")]
        public DateTime effective { get; set; }

        [JsonProperty("onset")]
        public DateTime? onset { get; set; }

        [JsonProperty("expires")]
        public DateTime expires { get; set; }

        [JsonProperty("ends")]
        public DateTime? ends { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("messageType")]
        public string messageType { get; set; }

        [JsonProperty("category")]
        public string category { get; set; }

        [JsonProperty("severity")]
        public string severity { get; set; }

        [JsonProperty("certainty")]
        public string certainty { get; set; }

        [JsonProperty("urgency")]
        public string urgency { get; set; }

        [JsonProperty("event")]
        public string @event { get; set; }

        [JsonProperty("sender")]
        public string sender { get; set; }

        [JsonProperty("senderName")]
        public string senderName { get; set; }

        [JsonProperty("headline")]
        public string headline { get; set; }

        [JsonProperty("description")]
        public string description { get; set; }

        [JsonProperty("instruction")]
        public string instruction { get; set; }

        [JsonProperty("response")]
        public string response { get; set; }

        [JsonProperty("parameters")]
        public Parameters parameters { get; set; }
    }
    public class Root
    {
        [JsonProperty("@context")]
        public List<object> context { get; set; }

        [JsonProperty("type")]
        public string type { get; set; }

        [JsonProperty("features")]
        public List<Feature> features { get; set; }

        [JsonProperty("title")]
        public string title { get; set; }

        [JsonProperty("updated")]
        public DateTime updated { get; set; }
    }
}
// Root class = JsonConvert.DeserializeObject<Root>(json);

