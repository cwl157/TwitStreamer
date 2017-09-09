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
    [Activity(Label = "TwitStreamer - Episode Details")]
    public class EpisodeDetailsActivity : Activity
    {
        private ShowViewModel _show;
        private EpisodeViewModel _episode;
        private RadioGroup _mediaTypeList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.EpisodeDetails);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Console.WriteLine("Resume");
            // Find selected data
            _show = TwitApi.Instance.Shows.FirstOrDefault(s => s.Selected == true);
            if (_show == null)
            {
                Android.Widget.Toast.MakeText(this, "Error finding show", Android.Widget.ToastLength.Short).Show();
                Intent i = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(i);
                return;
            }
            _episode = _show.Episodes.FirstOrDefault(e => e.Selected == true);
            if (_episode == null)
            {
                Android.Widget.Toast.MakeText(this, "Error finding episode", Android.Widget.ToastLength.Short).Show();
                Intent i = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(i);
                return;
            }

            // Setup UI elements
            Window.SetTitle(_show.Show.Label + " - " + _episode.Episode.Label);
            TextView titleTextView = FindViewById<TextView>(Resource.Id.txtvwEpisodeTitle);
            TextView descriptionTextView = FindViewById<TextView>(Resource.Id.txtvwEpisodeDescription);
            TextView episodeLengthTextView = FindViewById<TextView>(Resource.Id.txtvwEpisodeLength);

            titleTextView.Text = _episode.Episode.EpisodeNumber + " - " + _episode.Episode.Label;
            descriptionTextView.Text = _episode.Episode.Teaser;
            episodeLengthTextView.Text = _episode.Episode.AudioDetails.RunningTime.ToString() + " - " + _episode.Episode.AiringDate.ToLocalTime().ToString("M/d/yyyy");

            _mediaTypeList = FindViewById<RadioGroup>(Resource.Id.mediaTypeList);
            _mediaTypeList.RemoveAllViews();
            RadioButton audioButton = new RadioButton(this.ApplicationContext);
            RadioButton smallVideoButton = new RadioButton(this.ApplicationContext);
            RadioButton largeVideoButton = new RadioButton(this.ApplicationContext);
            RadioButton hdVideoButton = new RadioButton(this.ApplicationContext);
            audioButton.Text = "Audio";
            audioButton.SetPadding(5, 5, 0, 20);
            smallVideoButton.Text = "Small Video";
            smallVideoButton.SetPadding(5, 5, 0, 20);
            largeVideoButton.Text = "Large Video";
            largeVideoButton.SetPadding(5, 5, 0, 20);
            hdVideoButton.Text = "HD Video";
            hdVideoButton.SetPadding(5, 5, 0, 20);
            _mediaTypeList.AddView(audioButton);
            _mediaTypeList.AddView(smallVideoButton);
            _mediaTypeList.AddView(largeVideoButton);
            _mediaTypeList.AddView(hdVideoButton);

            // Setup button delegates
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
                            if (PlayingService.ServiceInstance.ShowId == _show.Show.Id)
                            {
                                if (PlayingService.ServiceInstance.EpisodeId != _episode.Episode.Id)
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
                int radioButtonID = _mediaTypeList.CheckedRadioButtonId;
                View radioButton = _mediaTypeList.FindViewById(radioButtonID);
                string videoUrl = "";
                int idx = _mediaTypeList.IndexOfChild(radioButton);
                Type activityToStart = typeof(PlayBackActivity);

                if (idx > 0)
                {
                    activityToStart = typeof(VideoViewerActivity);
                    if (idx == 1)
                    {
                        videoUrl = _episode.Episode.SmallVideoDetails.MediaUrl;
                    }
                    else if (idx == 2)
                    {
                        videoUrl = _episode.Episode.LargeVideoDetails.MediaUrl;
                    }
                    else if (idx == 3)
                    {
                        videoUrl = _episode.Episode.HdVideoDetails.MediaUrl;
                    }
                }
                Intent i = new Intent(this.ApplicationContext, activityToStart);
                if (videoUrl != "")
                {
                    i.PutExtra("VideoUrl", videoUrl);
                }
                StartActivity(i);
            }; // end play delegate

            Button download = FindViewById<Button>(Resource.Id.btnDownloadEpisode);
            download.Click += delegate
            {
                Android.Widget.Toast.MakeText(this, "Start Download", Android.Widget.ToastLength.Short).Show();
            };
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            Console.WriteLine("Restart");
        }
        protected override void OnStop()
        {
            base.OnStop();
        }
    }
}