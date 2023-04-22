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

            #region Recurring API call for data && expiration/CRUD logic

            #region API call, deserialization, and object creation
            var json = NwsApi.Callout("https://api.weather.gov/alerts/active/");
            var currentTime = DateTime.Now;
            Root NWSData = JsonConvert.DeserializeObject<Root>(json);
            var stormRepo = new StormRepository(conn);
            var currentStorms = stormRepo.GetAllStorms();
            bool newStorm = false;
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

                    foreach (var storm in currentStorms)
                    {
                        if (storm.id == props.properties.id && props.properties.messageType.ToLower() == "update")
                        {
                            stormRepo.UpdateStorm(temp);
                        }
                        else if (storm.id == props.properties.id && props.properties.messageType.ToLower() == "cancel")
                        {
                            stormRepo.DeleteStorm(temp);
                        }
                        else
                        {
                            newStorm = true;
                            stormRepo.CreateStorm(temp);
                        }
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