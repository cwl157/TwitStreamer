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
using TwitApi;
using TwitApi.Models;
using TwitApi.Methods;
using TwitStreamer.ViewModels;
using System.Threading.Tasks;

namespace TwitStreamer
{
    public sealed class TwitApi
    {
        private static readonly TwitApi instance = new TwitApi();
        private static readonly TwitApiService _apiService = new TwitApiService();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static TwitApi()
        {
        }

        private TwitApi()
        {
            Shows = new List<ShowViewModel>();
        }

        public static TwitApi Instance
        {
            get
            {
                return instance;
            }
        }

        public List<ShowViewModel> Shows { get; set; }

        public async Task RefreshShows()
        {
            foreach (ShowViewModel s in Shows)
            {
                s.Episodes.Clear();
            }

            Shows.Clear();

            IApiMethod showsApi = new GetShows();
            ShowsResponseModel apiResult = await _apiService.CallApi<ShowsResponseModel>(showsApi);

            foreach (ShowResponseModel s in apiResult.Shows)
            {
                Shows.Add(new ShowViewModel { Show = s });
            }
        }

        public async Task RefreshEpisodes(int id)
        {
            // TODO: Throw exception here if show is not found or if more than 1 show is found
            var show = Shows.SingleOrDefault(s => s.Show.Id == id);

            show.Episodes.Clear();

            GetEpisodesRequestModel request = new GetEpisodesRequestModel();
            request.ShowId = show.Show.Id;
            IApiMethod episodesApi = new GetEpisodes(request);
            EpisodesResponseModel apiResult = await _apiService.CallApi<EpisodesResponseModel>(episodesApi);

            foreach (EpisodeResponseModel e in apiResult.Episodes)
            {
                show.Episodes.Add(new EpisodeViewModel { Episode = e });
            }
        }

    }
}