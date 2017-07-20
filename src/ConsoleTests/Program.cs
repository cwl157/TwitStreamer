using System;
using System.Threading.Tasks;
using TwitApi.Models;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEpisodes();

            Console.Read();
        }

        private static async void TestEpisodes()
        {
            TwitApi.TwitApiService svc = new TwitApi.TwitApiService();
            var m = new TwitApi.Methods.GetEpisodes(new GetEpisodesRequestModel() { ShowId = 1642 });
            var result = await svc.CallApi<EpisodesResponseModel>(m);
            

          
                

                foreach (EpisodeResponseModel e in result.Episodes)
                {
                    Console.WriteLine(e.AudioDetails.Type + ": " + e.AudioDetails.MediaUrl);
                    Console.WriteLine(e.HdVideoDetails.Type + ": " + e.HdVideoDetails.MediaUrl);
                    Console.WriteLine(e.LargeVideoDetails.Type + ": " + e.LargeVideoDetails.MediaUrl);
                    Console.WriteLine(e.SmallVideoDetails.Type + ": " + e.SmallVideoDetails.MediaUrl);
                }

             

         //   task.Start();
         //   task.Wait();

            
        }
    }
}