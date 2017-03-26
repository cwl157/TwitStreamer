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
using TwitStreamer.Repositories;

namespace TwitStreamer
{
    [Activity(Label = "EpisodeDetailsActivity")]
    public class EpisodeDetailsActivity : Activity
    {
       // private string _showTitle;
        private EpisodeResponseModel _episode;
        private ShowResponseModel _show;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.EpisodeDetails);

            // Get our button from the layout resource,
            // and attach an event to it
            
                //int resId = Resources.GetIdentifier("txtvwEpisodeTitle", "TextView", "TwitStreamer");
            TextView titleTextView = FindViewById<TextView>(Resource.Id.txtvwEpisodeTitle);
            TextView descriptionTextView = FindViewById<TextView>(Resource.Id.txtvwEpisodeDescription);
            TextView episodeLengthTextView = FindViewById<TextView>(Resource.Id.txtvwEpisodeLength);

            //  string titleAndNumber = Intent.GetStringExtra("EpisodeId");
            //  string[] s = titleAndNumber.Split('-');
            //  string title = s[1].Trim();
            //  string num = s[0].Trim();
            // int episodeId = Intent.GetIntExtra("EpisodeId", 0);
            // int showId = Intent.GetIntExtra("ShowId", 0);
            int episodeId = -1;
            int showId = -1;
            //_showTitle = i.GetStringExtra("EpisodeName");

            if (savedInstanceState != null)
            {
                showId = savedInstanceState.GetInt("ShowId");
                episodeId = savedInstanceState.GetInt("EpisodeId");
            }
            else
            {
                showId = Intent.GetIntExtra("ShowId", -1);
                episodeId = Intent.GetIntExtra("EpisodeId", -1);
            }
            //episodeId = -1; // Debug Only
            if (showId == -1 || episodeId == -1)
            {
                Android.Widget.Toast.MakeText(this, "Error finding show or episode", Android.Widget.ToastLength.Short).Show();
                Intent i = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(i);
            }
            else
            {


                //_episode = TwitRepository.Episodes.Where(e => e.Label.ToLower() == title.ToLower()).First();
                _episode = TwitRepository.Episodes.Where(e => e.Id == episodeId).First();
                _show = TwitRepository.Shows.Where(s => s.Id == showId).First();
                titleTextView.Text = _episode.EpisodeNumber + " - " + _episode.Label;
                descriptionTextView.Text = _episode.Teaser;
                episodeLengthTextView.Text = _episode.AudioDetails.RunningTime.ToString() + " - " + _episode.AiringDate.ToLocalTime().ToString("M/d/yyyy");

                //_showTitle = Intent.GetStringExtra("ShowId");

                Button play = FindViewById<Button>(Resource.Id.btnPlayEpisode);
                play.Click += delegate
                {
                    // If an episode is already playing in a background, check to make sure the episode we clicked on is different, if it is, stop
                    // the current episode, saving its position, and start playing the new episode.
                    if (PlayingService.ServiceInstance != null)
                    {
                        if (PlayingService.ServiceInstance.Player != null)
                        {
                            if (PlayingService.ServiceInstance.Player.IsPlaying || PlayingService.ServiceInstance.Paused)
                            {
                                if (PlayingService.ServiceInstance.ShowId == _show.Id)
                                {
                                    if (PlayingService.ServiceInstance.EpisodeId != _episode.Id)
                                    {
                                        PlayingService.ServiceInstance.Stop();
                                    }
                                }
                                else // Show id is not equal, stop because it has to be a different episode even if episode id matches
                                {
                                    PlayingService.ServiceInstance.Stop();
                                }
                            }
                        }
                        
                    }
                    // Start playing episode
                    Intent i = new Intent(this.ApplicationContext, typeof(PlayBackActivity));
                    i.PutExtra("EpisodeId", _episode.Id);
                    i.PutExtra("ShowId", _show.Id);
                    //i.PutExtra("EpisodeName", titles[position]);
                    StartActivity(i);
                };
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
            if (_episode != null)
                outState.PutInt("EpisodeId", _episode.Id);
        }
    }
}