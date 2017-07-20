using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitApi;
using TwitApi.Methods;
using TwitApi.Models;

namespace UserTests
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEpisodes();

            Console.Read();
        }

        //private static void TwitApi_TestShows()
        //{
        //    TwitApiService service = new TwitApi.TwitApiService();

        //    Task<ShowsResponseModel> t = new Task<ShowsResponseModel>(() => { return service.CallApi<ShowsResponseModel>(new GetShows()).Result; });
        //    t.Start();
        //    t.Wait();

        //    foreach (ShowResponseModel s in t.Result.Shows)
        //    {
        //        Console.WriteLine(s.Id);
        //        Console.WriteLine(s.Label);
        //        Console.WriteLine(s.Description);
        //    }
        //}

        private static void TestEpisodes()
        {
            TwitApiService t = new TwitApiService();

            Task<EpisodesResponseModel> e = new Task<EpisodesResponseModel>(() => { return t.CallApi<EpisodesResponseModel>(new GetEpisodes(new GetEpisodesRequestModel { ShowId = 1642})).Result; });
            e.Start();
            e.ContinueWith((episodes) => {
                foreach (EpisodeResponseModel es in episodes.Result.Episodes)
                {
                    Console.WriteLine(es.Label);
                    foreach (VideoAudioDetailsResponseModel va in es.MediaTypes)
                    {
                        Console.WriteLine(va.Type);
                        Console.WriteLine(va.MediaUrl);
                        
                    }
                }
            });
        }

        //private static void TwitStreamer_TestShows()
        //{
        //    Task t = new Task(() => { TwitApi.Instance.RefreshShows(); });
        //    t.Start();
        //    t.Wait();

        //    foreach (TwitStreamer.ViewModels.ShowViewModel s in TwitStreamer.TwitApi.Instance.Shows)
        //    {
        //        Console.WriteLine(s.Show.Id);
        //        Console.WriteLine(s.Show.Label);
        //        Console.WriteLine(s.Show.Description);
        //    }
        //}
    }
}
