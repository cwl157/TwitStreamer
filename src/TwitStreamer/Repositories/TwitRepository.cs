using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TwitStreamer.Models;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using System;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace TwitStreamer.Repositories
{
    public class TwitRepository
    {
       

        // These are public static to be shared across activities
        public static List<ShowResponseModel> Shows = new List<ShowResponseModel>();
        public static List<EpisodeResponseModel> Episodes = new List<EpisodeResponseModel>();

     //   public TwitRepository()
     //   {
     //       Shows = new List<ShowResponseModel>();
     //       Episodes = new List<EpisodeResponseModel>();
     //   }

        public async Task<List<ShowResponseModel>> GetShows()
        {
            // return FetchShows(@"https://twit.tv/api/v1.0/shows?shows_active=1&fields=label,description");
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
                    string result = await response.Content.ReadAsStringAsync();
                    ShowsResponseModel showsFullResponse = JsonConvert.DeserializeObject<ShowsResponseModel>(result);
                    var shows = showsFullResponse.Shows;
                    return shows;
                    //Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                }
            }

            return null;
        }

        // Get episodes https://twit.tv/api/v1.0/episodes?filter[shows]=1665&fields=id,label,episodeNumber,teaser,airingDate
        public async Task<List<EpisodeResponseModel>> GetEpisodes(int showId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://twit.tv");
                //  client.GetAsync()
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("app-id", Keys.API_ID);
                client.DefaultRequestHeaders.Add("app-key", Keys.API_KEY);
                string endpoint = "/api/v1.0/episodes?filter[shows]={0}&fields=id,label,episodeNumber,teaser,airingDate,video_audio";
                endpoint = string.Format(endpoint, showId);
                HttpResponseMessage response = await client.GetAsync(endpoint);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    EpisodesResponseModel episodesFullResponse = JsonConvert.DeserializeObject<EpisodesResponseModel>(result);
                    var episodes = episodesFullResponse.Episodes;
                    return episodes;
                }

            }

            return null;
        }
    }
}