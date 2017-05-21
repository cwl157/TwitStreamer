using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitApi;
using TwitApi.Methods;
using TwitApi.Models;
using TwitStreamer.ViewModels;

namespace UserTests
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

        private List<ShowViewModel> _shows;

        public List<ShowViewModel> Shows { get { return _shows; } set { _shows = value; } }

        //public IEnumerable<ShowViewModel> Shows
        //{
        //    get
        //    {
        //        foreach (ShowViewModel s in _shows)
        //        {
        //            yield return s;
        //        }
        //    }
        //}

        public async void RefreshShows()
        {
            foreach (ShowViewModel s in _shows)
            {
                s.Episodes.Clear();
            }

            _shows.Clear();

            IApiMethod showsApi = new GetShows();
            ShowsResponseModel apiResult = await _apiService.CallApi<ShowsResponseModel>(showsApi);

            foreach (ShowResponseModel s in apiResult.Shows)
            {
                _shows.Add(new ShowViewModel { Show = s });
            }
        }

    }
}
