using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace WebAPIConsumer
{
    class Program
    {
        // Web Server Url API path 
        const string ServerUrl = "http://localhost:16555/api/Hotels";

        static void Main(string[] args)
        {
            //         

            CallWebApi().GetAwaiter().GetResult();
            System.Console.ReadKey();
        }

        static async Task CallWebApi()
        {
            // Step no 2 : Http client handler class for set default credentials 
            using (HttpClientHandler handler = new HttpClientHandler())
            {
                // set default credentials : authentication
                handler.UseDefaultCredentials = true;

                using (var client = new HttpClient(handler))
                {
                    // we must have to define base URI address
                    client.BaseAddress = new Uri(ServerUrl);
                    // we first we clear default header type then set content type application/Json as a default header type 
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // Send request for Get all information
                    // Step no 4: Call Get all Hotels here...................................
                    await GetAllHotels(client);
                    System.Console.WriteLine();
                    // Get By Id ...............................................
                    //System.Console.WriteLine("ENTER YOUR SPECIFIC ID");
                    //string id = System.Console.ReadLine();
                    await GetHotelById(client, "2");

                    // Post Method .....................................................
                    Hotel newHotel = new Hotel() { Name = "NewHotel", Address = "new address" , Hotel_No = 10};
                    await AddNewHotel(client, newHotel);
                }
            }
        }

        // Step no 3: Get all hotels   
        static async Task GetAllHotels(HttpClient client)
        {
            // GetString : Send a Get request to the specified URI and return the response body as a string in a asynchronous operation 
            var content = await client.GetStringAsync(ServerUrl);
            // Install Library Json.Net from NuGet Packet manager
            // Revert back Json data into objects
            var hotels = JsonConvert.DeserializeObject<IList<Hotel>>(content);
            foreach (var hotel in hotels)
            {
                System.Console.WriteLine(hotel);
            }
        }

        static async Task GetHotelById(HttpClient client, string id)
        {
            string urlId = ServerUrl + "/" + id;
            // we check also Http response message status
            HttpResponseMessage response = await client.GetAsync(urlId);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception("Customer not found , Try another Id");
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var hotel = JsonConvert.DeserializeObject<Hotel>(content);
            System.Console.WriteLine(
                $"HotelNo: {hotel.Hotel_No} , HotelName:{hotel.Name} , HotelAddress:{hotel.Address}");
        }

        // Post Method
        static async Task AddNewHotel(HttpClient client, Hotel newHotel)
        {
            // In Post Method first we convert New object into Json format
            // and format our Json string form
            var jsonStr = JsonConvert.SerializeObject(newHotel);
            System.Console.WriteLine("Json String:" + jsonStr);
            StringContent content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
            // send a Post request and Get response with newly added resource
            HttpResponseMessage response = await client.PostAsync(ServerUrl, content);
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new Exception("Hotel already exist:");
            }

            response.EnsureSuccessStatusCode();
            string str = await response.Content.ReadAsStringAsync();
            var hotel = JsonConvert.DeserializeObject<Hotel>(str);
            System.Console.WriteLine($"HotelNo: {hotel.Hotel_No} , HotelName:{hotel.Name} , HotelAddress:{hotel.Address}");

        }
    }
}
