using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TwitApi.Models;

namespace TwitApi.Methods
{
    public class GetEpisodes : IApiMethod
    {
        private GetEpisodesRequestModel _requestModel;
        public GetEpisodes(GetEpisodesRequestModel requestModel)
        {
            _requestModel = requestModel;
        }
        public async Task<string> Execute()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://twit.tv");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("app-id", Keys.API_ID);
                client.DefaultRequestHeaders.Add("app-key", Keys.API_KEY);

                string endpoint = "/api/v1.0/episodes?filter[shows]={0}&fields=id,label,episodeNumber,teaser,airingDate,video_audio";
                endpoint = string.Format(endpoint, _requestModel.ShowId);
                HttpResponseMessage response = await client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }

            }

            return "";
        }
    }
}
