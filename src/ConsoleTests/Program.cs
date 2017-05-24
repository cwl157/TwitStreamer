using System;
using System.Threading.Tasks;
using TwitApi.Models;

namespace ConsoleTests
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEpisodes()
        }

        private static void TestEpisodes()
        {
            Task<EpisodesResponseModel> task = new Task<EpisodesResponseModel>(() => {

            });
        }
    }
}