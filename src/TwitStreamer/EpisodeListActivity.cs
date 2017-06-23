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
using TwitStreamer.ViewModels;

namespace TwitStreamer
{
    [Activity(Label = "TwitStreamer - Episode List")]
    public class EpisodeListActivity : ListActivity
    {
        List<string> _titles;
        private ShowViewModel _show;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _titles = new List<string>();
            _titles.Add("Refresh");
            ListAdapter = new ShowListViewAdatper(this, _titles.ToArray());
            
        }

        protected override void OnResume()
        {
            base.OnResume();

            _show = TwitApi.Instance.Shows.FirstOrDefault(s => s.Selected == true);
            
            if (_show == null)
            {
                Android.Widget.Toast.MakeText(this, "Error finding show", Android.Widget.ToastLength.Short).Show();
                Intent intent = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(intent);
                return;
            }
            Window.SetTitle(_show.Show.Label + " - Episodes");
            _titles.Clear();

            if (_show.Episodes.Count > 0)
            {
                foreach (EpisodeViewModel e in _show.Episodes)
                {
                    _titles.Add(e.Episode.EpisodeNumber + " - " + e.Episode.Label);
                }
            }
            _titles.Insert(0, "Refresh");
            ListView.Adapter = new ShowListViewAdatper(this, _titles.ToArray());
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            if (position == 0) // Refresh will always be the first item
                GetEpisodes();
            else // Clicked on a show, Go to Show details page
            {
                var episode = _show.Episodes.FirstOrDefault(e => e.Episode.EpisodeNumber + " - " + e.Episode.Label == _titles[position]);
                if (episode != null)
                {
                    _show.Episodes.ForEach(e => e.Selected = false);
                    episode.Selected = true;
                }
                Intent i = new Intent(this.ApplicationContext, typeof(EpisodeDetailsActivity));
                StartActivity(i);
            }
        }

        private async void GetEpisodes()
        {
            await TwitApi.Instance.RefreshEpisodes(_show.Show.Id);

           if (_show.Episodes.Count > 0)
            {
                _titles.Clear();
                foreach (EpisodeViewModel e in _show.Episodes)
                {
                    _titles.Add(e.Episode.EpisodeNumber+" - "+e.Episode.Label);
                }
                _titles.Insert(0, "Refresh");
                ListView.Adapter = new ShowListViewAdatper(this, _titles.ToArray());
            }
            else
            {
                Android.Widget.Toast.MakeText(this, "Failed To Get List Of Episodes", Android.Widget.ToastLength.Short).Show();
            }
        }
    }
}