using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TwitStreamer.Repositories;
using System.Threading.Tasks;
using TwitStreamer.Models;
using System.Collections.Generic;

namespace TwitStreamer
{
    [Activity(Label = "TwitStreamer", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
        private List<string> _titles;
        private TwitRepository _repo;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _repo = new TwitRepository();
            _titles = new List<string>();
            _titles.Add("Refresh");
            ListAdapter = new ShowListViewAdatper(this, _titles.ToArray());
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            if (position == 0) // Refresh will always be the first item
                GetShows(); 
            else // Clicked on a show, Go to Episode ListActivity
            {
                Intent i = new Intent(this.ApplicationContext, typeof(EpisodeListActivity));
                var show = TwitRepository.Shows.Find(s => s.Label == _titles[position]);
                if (show != null)
                    i.PutExtra("ShowId", show.Id);
                else
                    i.PutExtra("ShowId", -1);
                StartActivity(i);
            }
        }

        private async void GetShows()
        {
          //  if (TwitRepository.Shows.Count == 0)
            TwitRepository.Shows = await _repo.GetShows();
            if (TwitRepository.Shows != null)
            {
                _titles.Clear();
                foreach (ShowResponseModel s in TwitRepository.Shows)
                {
                    _titles.Add(s.Label);
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

