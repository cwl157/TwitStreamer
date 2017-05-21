using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TwitApi.Models.Api;

namespace TwitApi.Methods
{
    public class GetShows : IApiMethod
    {
        public async Task<string> Execute()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://twit.tv");
                //  client.GetAsync()
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("app-id", Keys.API_ID);
                client.DefaultRequestHeaders.Add("app-key", Keys.API_KEY);


                // New code:
                HttpResponseMessage response = await client.GetAsync("/api/v1.0/shows?shows_active=1&fields=label,description,id");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }

            return "";
        }
    }
}
