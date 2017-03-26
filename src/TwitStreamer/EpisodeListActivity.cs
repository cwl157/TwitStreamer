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
using TwitStreamer.Repositories;
using TwitStreamer.Models;

namespace TwitStreamer
{
    [Activity(Label = "EpisodeListActivity")]
    public class EpisodeListActivity : ListActivity
    {
        List<string> _titles;
        // private string _showTitle;
        private ShowResponseModel _show;
        TwitRepository _repo;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _repo = new TwitRepository();
            // Create your application here
            _titles = new List<string>();
            _titles.Add("Refresh");
            ListAdapter = new ShowListViewAdatper(this, _titles.ToArray());

            Intent i = Intent;

            //_showTitle = i.GetStringExtra("EpisodeName");
            int showId = -1;

            if (savedInstanceState != null)
            {
                showId = savedInstanceState.GetInt("ShowId");
            }
            else
            {
                showId = i.GetIntExtra("ShowId", -1);
            }
            // showId = -1; Debug only
            if (showId == -1)
            {
                Android.Widget.Toast.MakeText(this, "Error finding show", Android.Widget.ToastLength.Short).Show();
                Intent intent = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(intent);
            }

            else
            {
                _show = TwitRepository.Shows.Find(s => s.Id == showId);
            }
            
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (_show != null)
                outState.PutInt("ShowId", _show.Id);
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            //  TwitRepository repo = new TwitRepository();
            //  repo.GetShows();
            if (position == 0) // Refresh will always be the first item
                GetEpisodes();
            else // Clicked on a show, Go to Show details page
            {
                //var episode = TwitRepository.Episodes.Find(s => s.Label == _titles[position]);
                //   var episodes = TwitRepository.Episodes;
                //episodes[0].EpisodeNumber + " - " + episodes[0].Label == _titles[position]
                var episode = TwitRepository.Episodes.FirstOrDefault(s => s.EpisodeNumber+" - "+s.Label == _titles[position]);
                Intent i = new Intent(this.ApplicationContext, typeof(EpisodeDetailsActivity));
                i.PutExtra("EpisodeId", episode.Id);
                i.PutExtra("ShowId", _show.Id);
                StartActivity(i);
            }
            //     var t = items[position];
            //   Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Short).Show();
        }

        private async void GetEpisodes()
        {
            //   TwitRepository repo = new TwitRepository();
            int showId = TwitRepository.Shows.Where(s => s.Label == _show.Label).Select(i => i.Id).First();
          //  if (TwitRepository.Episodes.Count == 0)
            TwitRepository.Episodes = await _repo.GetEpisodes(showId);
            if (TwitRepository.Episodes != null)
            {
                _titles.Clear();
                //        List<string> titles = new List<string>();
                foreach (EpisodeResponseModel s in TwitRepository.Episodes)
                {
                    _titles.Add(s.EpisodeNumber+" - "+s.Label);
                }
                _titles.Insert(0, "Refresh");
                ListView.Adapter = new ShowListViewAdatper(this, _titles.ToArray());
            }
            else
            {
                Android.Widget.Toast.MakeText(this, "Failed To Get List Of Shows", Android.Widget.ToastLength.Short).Show();
            }
        }
    }
}