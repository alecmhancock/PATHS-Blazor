using Newtonsoft.Json;
using PATHSMap.Data;
using dymaptic.GeoBlazor.Core;
using System.Data;
using System.Data.SqlClient;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PATHSMap
{
    public class Program
    {
        private static System.Timers.Timer _timer;
        public static async Task Main(string[] args)
        {

            #region SQL Server Connection
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connString = config.GetConnectionString("DefaultConnection");
            IDbConnection conn = new SqlConnection(connString);
            #endregion

            #region Web client params
            var client = new HttpClient();
            new NwsApi(client);

            #endregion

            #region Timer to control API calls
            _timer = new System.Timers.Timer(5000); // set the interval to 5 seconds (TEST VALUE ONLY)
            _timer.Elapsed += async (sender, e) => await RunApiCall(conn);
            _timer.Start();
            #endregion

            #region Blazor Startup
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddGeoBlazor();
            var app = builder.Build();
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
            app.Run();
            #endregion
        }
        private static async Task RunApiCall(IDbConnection conn)
        {
            #region Recurring API call for data && expiration/CRUD logic

            #region API call, deserialization, and object creation
            var json = NwsApi.Callout("https://api.weather.gov/alerts/active/");
            var currentTime = DateTime.Now;
            Root NWSData = JsonConvert.DeserializeObject<Root>(json);
            var stormRepo = new StormRepository(conn);
            var currentStorms = stormRepo.GetAllStorms();
            #endregion

            #region Loop through all storms returned

            foreach (var props in NWSData.features)
            {   //logic to remove test events, empty messages, or expired events.
                var temp = new Storm();
                if (props.properties.expires < currentTime || props.properties.headline == null || props.properties.@event == "Test Message")
                {
                    continue;
                }
                if (props.geometry == null)
                {
                    continue;
                }
                if (props.properties.@event == "Severe Thunderstorm Warning" || props.properties.@event == "Tornado Warning")
                {
                    #region logic to break down the list<list<list<double>>> hierarchy defined by the API for coordinates

                    var coordlist = props.geometry.coordinates
                    .SelectMany(list1 => list1)
                    .SelectMany(items => items)
                    .ToList();

                    #endregion

                    #region Assigning temporary object properties

                    temp.headline = props.properties.headline;
                    temp.id = props.properties.id;
                    temp.expiration = props.properties.expires;
                    temp.areaDesc = props.properties.areaDesc;
                    temp.description = props.properties.description;
                    temp.messageType = props.properties.messageType;
                    var motionString = props.properties.parameters.eventMotionDescription.ToString();
                    temp.@event = props.properties.@event;

                    #endregion

                    #region CRUD functions for SQL database

                    var stormToUpdate = currentStorms.FirstOrDefault(storm => storm.id == props.properties.id && props.properties.messageType.ToLower() == "update");
                    var stormToCancel = currentStorms.FirstOrDefault(storm => storm.id == props.properties.id && props.properties.messageType.ToLower() == "cancel");

                    if (stormToUpdate != null)
                    {
                        stormRepo.UpdateStorm(temp);
                    }
                    else if (stormToCancel != null)
                    {
                        stormRepo.DeleteStorm(temp);
                    }
                    else
                    {
                        
                        stormRepo.CreateStorm(temp);
                    }
                    
                    #endregion
                }
                
            }
            #endregion

            #region Expiration logic for storms that are no longer active
            foreach (var storm in currentStorms)
            {
                if (storm.expiration < currentTime)
                {
                    stormRepo.DeleteStorm(storm);
                }
            }
            
            #endregion

            #endregion
        }
    }
}