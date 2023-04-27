using Newtonsoft.Json;
using PATHSMap.Data;
using System.Data;
using System.Data.SqlClient;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Data.SqlTypes;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
using MudBlazor.Services;

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
            new NWSAPI(client);

            #endregion

            #region Timer to control API calls
            _timer = new System.Timers.Timer(5000); // set the interval to 30 seconds (TEST VALUE ONLY)
            _timer.Elapsed += async (sender, e) => await RunApiCall(conn);
            _timer.Start();
            #endregion

            #region Blazor Startup
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMudServices();
            builder.Logging.AddConsole();
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
            var json = NWSAPI.CalloutNWS("https://api.weather.gov/alerts/active/");
            var json2 = File.ReadAllText(@"C:\Users\Administrator\Documents\Programming\PATHS\PATHSMap\temp.json");

            var currentTime = DateTime.Now;

            Root NWSData = JsonConvert.DeserializeObject<Root>(json2);
            var stormRepo = new StormRepository(conn);
            
            #endregion

            #region Loop through all storms returned

            foreach (var props in NWSData.features)
            {   //logic to remove test events, empty messages, or expired events.
                var temp = new Storm();
                //if (props.properties.expires < currentTime || props.properties.headline == null || props.properties.@event == "Test Message")
                //{
                //    continue;
                //}
                if (props.properties.@event == "Severe Thunderstorm Warning" || props.properties.@event == "Tornado Warning")
                {
                    #region logic to break down the list<list<list<double>>> hierarchy defined by the API for coordinates
                    var references = props.properties.references.ToString();

                    

                    #endregion

                    #region Assigning temporary object properties

                    temp.headline = props.properties.headline;
                    temp.id = props.properties.id;
                    temp.expiration = props.properties.expires;
                    temp.areaDesc = props.properties.areaDesc;
                    temp.description = props.properties.description;
                    temp.messageType = props.properties.messageType;
                    
                    temp.eventType = props.properties.@event;
                    temp.sent = props.properties.sent;
                    foreach (var reference in props.properties.references)
                    {
                        temp.refid = reference.id;
                    }


                    #endregion

                    #region CRUD functions for SQL database

                    var existingStorm = stormRepo.GetStormById(temp.id);

                    if (existingStorm == null)
                    {
                        stormRepo.CreateStorm(temp);
                    }
                    else
                    {
                        if (temp.messageType.ToLower() == "update" && temp.refid == existingStorm.id)
                        {
                            stormRepo.UpdateStorm(temp);
                        }
                        else if (temp.messageType.ToLower() == "cancel" && temp.refid == existingStorm.id)
                        {
                            stormRepo.DeleteStorm(existingStorm);
                        }
                        else if (temp.messageType.ToLower() == "alert" && temp.id == existingStorm.id)
                        {
                            stormRepo.DeleteStorm(existingStorm);
                            stormRepo.CreateStorm(temp);
                        }
                    }


                    #endregion
                }

            }
            #endregion

            #region Expiration logic for storms that are no longer active
            //var currentStorms = stormRepo.GetAllStorms();
            //foreach (var storm in currentStorms)
            //{
            //    if (storm.expiration < currentTime)
            //    {
            //        stormRepo.DeleteStorm(storm);
            //    }
            //}

            #endregion

            #endregion


        }
    }
}