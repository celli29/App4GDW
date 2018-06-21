using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Geolocator;

namespace App4GDW
{
    public class RestService : IRestService
    {
        HttpClient client;

        public List<SimpleCourse> Items { get; private set; }
        public List<SimpleTee> TeeItems { get; private set; }
        public List<SimpleCoordinates> CoordinatesItems { get; private set; }
        public List<TeeCommonInfoes> TeeInfoItems { get; private set; }

        public RestService()
        {
            // to be modified if it is used for me
            var authData = string.Format("{0}:{1}", Constants.Username, Constants.Password);
            var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));

            client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;
        }

        public async Task<List<SimpleCourse>> RefreshCourseDataAsync()
        {
            Items = new List<SimpleCourse>();

            // from plugin.geolocator
            //var timeout = TimeSpan.FromSeconds(1);
            //var locator = CrossGeolocator.Current;
            //locator.DesiredAccuracy = 50;
            //var currentMyPosition = await locator.GetPositionAsync(timeout, null, true);
            //var currentMyPosition = new Plugin.Geolocator.Position(33.78, -97.55);


            string apiUrl = "http://api.golfdataworld.com/api/Course/";

            try
            {
                var response = await client.GetAsync(apiUrl);
                if(response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Items = JsonConvert.DeserializeObject<List<SimpleCourse>>(content);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(@"     Error {0}", ex.Message);
            }

            return Items;
        }

        public async Task<List<SimpleTee>> RefreshTeeDataAsync(int gcid)
        {
            TeeItems = new List<SimpleTee>();

            string apiUrl = "http://api.golfdataworld.com/api/Course?gcid=" + gcid;

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    TeeItems = JsonConvert.DeserializeObject<List<SimpleTee>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"     Error {0}", ex.Message);
            }

            return TeeItems;
        }


        // to get all the coordinates at the course (gcid)
        public async Task<List<SimpleCoordinates>> RefreshCoordinatesDataAsync(int gcid)
        {
            CoordinatesItems = new List<SimpleCoordinates>();

            string apiUrl = "http://api.golfdataworld.com/api/Coordinate?gcid=" + gcid;

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    CoordinatesItems = JsonConvert.DeserializeObject<List<SimpleCoordinates>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"     Error {0}", ex.Message);
            }

            return CoordinatesItems;
        }

        // to get three lines from teecommon info that includes par, handicap, and distance
        // RefreshTeeInfoDataAsync
        public async Task<List<TeeCommonInfoes>> RefreshTeeInfoDataAsync(int gcid, string name, string gender)
        {
            TeeInfoItems = new List<TeeCommonInfoes>();

            string apiUrl = "http://api.golfdataworld.com/api/TeeInfo?gcid=" + gcid + "&name=" + name + "&gender=" + gender;

            try
            {
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    TeeInfoItems = JsonConvert.DeserializeObject<List<TeeCommonInfoes>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"     Error {0}", ex.Message);
            }

            return TeeInfoItems;
        }

    }
}
