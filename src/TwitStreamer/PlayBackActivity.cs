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
using Android.Media;
using TwitStreamer.Repositories;
using TwitStreamer.Models;
using System.Threading.Tasks;

namespace TwitStreamer
{
    /* TODO
     *
     * Create buttons to fast forward and rewind
     * Create widget that runs in the notification bar with playback controls on it
     */

    [Activity(Label = "PlayBackActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleTask)]
    public class PlayBackActivity : Activity
    {
        private const string SAVE_FILE_NAME = "PlayBackData";
        private Button _playButton;
       // private MediaPlayer _mediaPlayer;
        private SeekBar _seekBar;
        private TimeSpan _mediaLength;
        private Button _stopButton;
        private Button _pauseButton;
       // private Button _updateSeekBar;
        private EpisodeResponseModel _episode;
        private ShowResponseModel _show;
        private string _showTitle;
        private string _mediaUri;
        private TextView _playBackProgressText;
        private TextView _playBackDurationText;
        private System.Timers.Timer _updateTimer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.PlayBack);
            _seekBar = FindViewById<SeekBar>(Resource.Id.skbarPlayBack);
            int episodeId = -1;
            int showId = -1;
            //_showTitle = i.GetStringExtra("EpisodeName");

            // 3 ways to get current show and episode at this point
            // 1: See if there's a saved bundle and grab from there. This would be if this particular activity is stopped and then gone back into, could grab from the bundle
            if (savedInstanceState != null)
            {
                showId = savedInstanceState.GetInt("ShowId");
                episodeId = savedInstanceState.GetInt("EpisodeId");
            }
            else
            {
                // 2. If the bundle doesn't work, try grabbing from the intent. This is used when this activity is started from the EpisodeListActivity
                // during the normal application path.
                showId = Intent.GetIntExtra("ShowId", -1);
                episodeId = Intent.GetIntExtra("EpisodeId", -1);
                if (showId == -1 || episodeId == -1)
                {
                    // 3. If its not in the intent from the EpisodeListActivity, get it from the playing service.
                    if (PlayingService.ServiceInstance != null)
                    {
                        showId = PlayingService.ServiceInstance.ShowId;
                        episodeId = PlayingService.ServiceInstance.EpisodeId;
                    }
                }
            }
            // If we can't get it from the saved bundle, the intent coming from the EpisodeListActivity, or the playing service, consider them lost and
            // kick to the beginning of the app to choose a show and episode (It might be possible to get here and have the service still be playing, if that's the case
            // the user will just need to return to the episode they were listing to
            if (showId == -1 || episodeId == -1)
            {
                Android.Widget.Toast.MakeText(this, "Error finding show or episode", Android.Widget.ToastLength.Short).Show();
                Intent i = new Intent(this.ApplicationContext, typeof(MainActivity));
                StartActivity(i);
            }

            /*  if (PlayingService.ServiceInstance != null)
              {
                  if (PlayingService.ServiceInstance.CurrentlyPlayingEpisodeId != id)
                  {
                      if (PlayingService.ServiceInstance.Player != null)
                      {
                          if (PlayingService.ServiceInstance.Player.IsPlaying || PlayingService.ServiceInstance.Paused)
                          {
                              PlayingService.ServiceInstance.Stop();
                          }
                      }
                  }
              } 
              */
            //else
            //{
                _episode = TwitRepository.Episodes.FirstOrDefault(e => e.Id == episodeId);
                _show = TwitRepository.Shows.FirstOrDefault(s => s.Id == showId);


                TextView episodeHeading = FindViewById<TextView>(Resource.Id.txtVwEpisodeTitle);
                episodeHeading.Text = _episode.EpisodeNumber + " - " + _episode.Label;

                _showTitle = _show.Label;

                TextView showTitleView = FindViewById<TextView>(Resource.Id.txtvwShowTitle);
                showTitleView.Text = _showTitle;
                _playBackProgressText = FindViewById<TextView>(Resource.Id.txtPlayBackProgress);
                _playBackDurationText = FindViewById<TextView>(Resource.Id.txtPlayBackDuration);
                _playBackDurationText.Text = "0";
                _playBackProgressText.Text = "0";

                _mediaLength = new TimeSpan(_episode.AudioDetails.Hours, _episode.AudioDetails.Minutes, _episode.AudioDetails.Seconds);

                _mediaUri = _episode.AudioDetails.MediaUrl;

                // _seekBar.Max = GetDuration();
                TimeSpan t = new TimeSpan(_episode.AudioDetails.Hours, _episode.AudioDetails.Minutes, _episode.AudioDetails.Seconds);
                _seekBar.Max = int.Parse(t.TotalMilliseconds.ToString());
                _playBackDurationText.Text = GetTimeString(_seekBar.Max);

                _seekBar.Progress = GetCurrentPosition();
                _playBackProgressText.Text = GetTimeString(_seekBar.Progress);

                _playButton = FindViewById<Button>(Resource.Id.btnPlay);
                _stopButton = FindViewById<Button>(Resource.Id.btnStop);
                _pauseButton = FindViewById<Button>(Resource.Id.btnPause);
                // _updateSeekBar = FindViewById<Button>(Resource.Id.btnUpdateSeekBar);

                _playButton.Click += (sender, args) => SendAudioCommand(new PlayingServiceInputModel() { Action = PlayingService.ActionPlay, SeekBarPosition = _seekBar.Progress });
                _pauseButton.Click += (sender, args) => SendAudioCommand(new PlayingServiceInputModel() { Action = PlayingService.ActionPause, SeekBarPosition = 0 });
                _stopButton.Click += (sender, args) => SendAudioCommand(new PlayingServiceInputModel() { Action = PlayingService.ActionStop, SeekBarPosition = 0 });
                //    _updateSeekBar.Click += (sender, args) => UpdateSeekBar_Click(sender, args);

                _updateTimer = new System.Timers.Timer(1000);
                _updateTimer.Elapsed += (sender, e) =>
                {
                    _seekBar.Progress = GetCurrentPositionIncremental();
                    RunOnUiThread(() =>
                    {
                        _playBackProgressText.Text = GetTimeString(_seekBar.Progress);
                    });
                };

                _seekBar.ProgressChanged += (object sender, SeekBar.ProgressChangedEventArgs e) =>
                {
                    if (e.FromUser)
                    {
                        _playBackProgressText.Text = GetTimeString(_seekBar.Progress);
                        if (PlayingService.ServiceInstance != null)
                        {
                            if (PlayingService.ServiceInstance.Player != null)
                            {
                                PlayingService.ServiceInstance.Player.SeekTo(_seekBar.Progress);
                            }
                        }
                    }
                };
           // }
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (PlayingService.ServiceInstance != null)
            {
                if (PlayingService.ServiceInstance.Player != null)
                {
                    if (PlayingService.ServiceInstance.Player.IsPlaying)
                    {
                        _updateTimer.Start();
                    }
                }
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            if (_show != null)
                outState.PutInt("ShowId", _show.Id);
            if (_episode != null)
                outState.PutInt("EpisodeId", _episode.Id);
        }

        //private void SendAudioCommand(string action)
        private void SendAudioCommand(PlayingServiceInputModel model)
        {
            string action = model.Action;
            if (action == PlayingService.ActionPlay)
            {
                _updateTimer.Start();
            }
            else if (action == PlayingService.ActionPause || action == PlayingService.ActionStop)
            {
                _updateTimer.Stop();
            }
            var intent = new Intent(action);
            intent.PutExtra("SeekBarPosition", model.SeekBarPosition);
            intent.PutExtra("mediaUri", _mediaUri);
            string mediaStreamingTitle = _showTitle + ": " + _episode.EpisodeNumber + " - " + _episode.Label;
            intent.PutExtra("streamingDetails", mediaStreamingTitle);
            intent.PutExtra("EpisodeId", _episode.Id);
            intent.PutExtra("ShowTitle", _showTitle);
            intent.PutExtra("ShowId", _show.Id);
            StartService(intent);
        }

        protected override void OnStop()
        {
            if (_updateTimer != null)
                _updateTimer.Stop();
            base.OnStop();
        }

        private int GetDuration()
        {
            int result = 100;
            if (PlayingService.ServiceInstance != null)
            {
                if (PlayingService.ServiceInstance.Player != null)
                {
                    int duration = PlayingService.ServiceInstance.Player.Duration;
                    result = duration;
                }
            }

            return result;
        }

        private int GetCurrentPosition()
        {
            int result = _seekBar.Progress;
            if (PlayingService.ServiceInstance != null)
            {
                if (PlayingService.ServiceInstance.Player != null)
                {
                    if (PlayingService.ServiceInstance.Player.IsPlaying || PlayingService.ServiceInstance.Paused)
                    {
                        int currentSpot = PlayingService.ServiceInstance.Player.CurrentPosition;
                        result = currentSpot;
                    }
                    else // If its not playing or paused, try to get any saved position
                    {
                        result = GetPlayBackPosition();
                    }
                }
                else
                {
                    result = GetPlayBackPosition();
                }
            }
            else
            {
                result = GetPlayBackPosition();
            }

            return result;
        }

        private int GetCurrentPositionIncremental()
        {
            int result = _seekBar.Progress;
            if (PlayingService.ServiceInstance != null)
            {
                if (PlayingService.ServiceInstance.Player != null)
                {
                    if (PlayingService.ServiceInstance.Player.IsPlaying || PlayingService.ServiceInstance.Paused)
                    {
                        int currentSpot = PlayingService.ServiceInstance.Player.CurrentPosition;
                        result = currentSpot;
                    }
                   // else // If its not playing or paused, try to get any saved position
                   // {
                   //     result = GetPlayBackPosition();
                   // }
                }
                else
                {
                    result = GetPlayBackPosition();
                }
            }
            else
            {
                result = GetPlayBackPosition();
            }

            return result;
        }

        private int GetPlayBackPosition()
        {
            int result = 0;
           // var show = TwitRepository.Shows.FirstOrDefault(s => s.Label == _showTitle);
           // var episode = TwitRepository.Episodes.FirstOrDefault(e => e.Id == _episodeId);
           // Use both local variables here instead of finding show from Repo based on title
            string saveKey = _show.Id + "|" + _episode.Id + "|PlayBackPosition";
            var sharedPref = this.GetSharedPreferences(SAVE_FILE_NAME, FileCreationMode.Private);
            int defaultValue = result;

            result = sharedPref.GetInt(saveKey, defaultValue);

            if (result < 0) result = 0;

            return result;
        }

        private string GetTimeString(int time)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(time);
            //string progressTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
            string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    ts.Hours,
                                    ts.Minutes,
                                    ts.Seconds);
            return timeString;
        }

      //  private void UpdateSeekBar_Click(object sender, EventArgs e)
     //   private void UpdateSeekBar()
       // {
       //     _seekBar.Progress = GetCurrentPosition();
       //     _playBackProgressText.Text = GetTimeString(_seekBar.Progress);
       // }

    
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}