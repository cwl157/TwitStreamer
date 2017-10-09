using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using System.Collections.Generic;
using TwitStreamer.ViewModels;

namespace TwitStreamer
{
    [Activity(Label = "TwitStreamer - Show List", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : ListActivity
    {
        private List<string> _titles;
        private ProgressDialog _progress;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _progress = new ProgressDialog(this);
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
                var show = TwitApi.Instance.Shows.Find(s => s.Show.Label == _titles[position]);
                if (show != null)
                {
                    TwitApi.Instance.Shows.ForEach(s => s.Selected = false);
                    show.Selected = true;
                }

                StartActivity(i);
            }
        }

        private async void GetShows()
        {
            _progress.SetMessage("Downloading show list...");
            _progress.SetCancelable(false);
            _progress.Show();
            

            await TwitApi.Instance.RefreshShows();

            _progress.Hide();

            if (TwitApi.Instance.Shows.Count > 0)
            {
                _titles.Clear();
                foreach (ShowViewModel s in TwitApi.Instance.Shows)
                {
                    _titles.Add(s.Show.Label);
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

