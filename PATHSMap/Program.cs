using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using PATHSMap.Data;
using System.Net.Http.Headers;
using CsvHelper;
using PATHSMap;
using System.Formats.Asn1;
using System.Globalization;
using System.Net.Http.Headers;
using dymaptic.GeoBlazor.Core;
using Microsoft.AspNetCore.Authentication;
using System.Data;
using System.Data.SqlClient;

namespace PATHSMap
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var currentTime = DateTime.Now;

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
            var json = NwsApi.Callout("https://api.weather.gov/alerts/active/");
            #endregion

            #region API call for data
            //Naming parameter "json" will be live data, naming parameter "json2" will be test data.
            //Make sure to comment out first "if" statement if test data is being utilized.
            Root NWSData = JsonConvert.DeserializeObject<Root>(json);
            //Root NWSData = JsonConvert.DeserializeObject<Root>(json2);
            foreach (var props in NWSData.features)
            {   //logic to remove test events, empty messages, or expired events.
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
                    var coordlist = new List<double>();
                    foreach (var list1 in props.geometry.coordinates)
                    {
                        foreach (var items in list1)
                        {
                            foreach (var coords in items)
                                coordlist.Add(coords);
                        }
                    }
                    #endregion
                    var stormRepo = new StormRepository(conn);
                    var temp = new Storm();
                    temp.headline = props.properties.headline;
                    temp.id = props.properties.id;
                    temp.expiration = props.properties.expires;
                    temp.areaDesc = props.properties.areaDesc;
                    temp.description = props.properties.description;
                    temp.messageType = props.properties.messageType;
                    var motionString = props.properties.parameters.eventMotionDescription.ToString();
                    temp.eventType = props.properties.@event;
                    stormRepo.CreateStorm(props.properties.id, props.properties.headline,
                        props.properties.areaDesc, props.properties.expires, props.properties.description,
                        props.properties.messageType, motionString, props.properties.@event);
                }

            }
            #endregion

            #region Blazor Startup
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddGeoBlazor();
            var app = builder.Build();
            // Configure the HTTP request pipeline.
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
    }
}