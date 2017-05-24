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
using TwitStreamer.InputModels;
using System.Threading.Tasks;
using TwitStreamer.ViewModels;

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
        private SeekBar _seekBar;
        private TimeSpan _mediaLength;
        private Button _stopButton;
        private Button _pauseButton;
        private EpisodeViewModel _episode;
        private ShowViewModel _show;
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

            if (PlayingService.ServiceInstance != null)
            {
                if (PlayingService.ServiceInstance.Player != null)
                {
                    if (PlayingService.ServiceInstance.Player.IsPlaying || PlayingService.ServiceInstance.Paused)
                    {
                        TwitApi.Instance.Shows.ForEach(s => s.Selected = false);
                        var playingShow = TwitApi.Instance.Shows.FirstOrDefault(s => s.Show.Id == PlayingService.ServiceInstance.ShowId);
                        if (playingShow != null)
                        {
                            playingShow.Selected = true;


                            var currentEpisode = playingShow.Episodes.FirstOrDefault(e => e.Episode.Id == PlayingService.ServiceInstance.EpisodeId);
                            if (currentEpisode != null)
                            {
                                playingShow.Episodes.ForEach(e => e.Selected = false);
                                currentEpisode.Selected = true;
                            }
                        }
                    }
                }
            }

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

            TextView episodeHeading = FindViewById<TextView>(Resource.Id.txtVwEpisodeTitle);
            episodeHeading.Text = _episode.Episode.EpisodeNumber + " - " + _episode.Episode.Label;

            _showTitle = _show.Show.Label;

            TextView showTitleView = FindViewById<TextView>(Resource.Id.txtvwShowTitle);
            showTitleView.Text = _showTitle;
            _playBackProgressText = FindViewById<TextView>(Resource.Id.txtPlayBackProgress);
            _playBackDurationText = FindViewById<TextView>(Resource.Id.txtPlayBackDuration);
            _playBackDurationText.Text = "0";
            _playBackProgressText.Text = "0";

            _mediaLength = new TimeSpan(_episode.Episode.AudioDetails.Hours, _episode.Episode.AudioDetails.Minutes, _episode.Episode.AudioDetails.Seconds);

            _mediaUri = _episode.Episode.AudioDetails.MediaUrl;

            TimeSpan t = new TimeSpan(_episode.Episode.AudioDetails.Hours, _episode.Episode.AudioDetails.Minutes, _episode.Episode.AudioDetails.Seconds);
            _seekBar.Max = int.Parse(t.TotalMilliseconds.ToString());
            _playBackDurationText.Text = GetTimeString(_seekBar.Max);

            _seekBar.Progress = GetCurrentPosition();
            _playBackProgressText.Text = GetTimeString(_seekBar.Progress);

            _playButton = FindViewById<Button>(Resource.Id.btnPlay);
            _stopButton = FindViewById<Button>(Resource.Id.btnStop);
            _pauseButton = FindViewById<Button>(Resource.Id.btnPause);

            _playButton.Click += (sender, args) => SendAudioCommand(new PlayingServiceInputModel() { Action = PlayingService.ActionPlay, SeekBarPosition = _seekBar.Progress });
            _pauseButton.Click += (sender, args) => SendAudioCommand(new PlayingServiceInputModel() { Action = PlayingService.ActionPause, SeekBarPosition = 0 });
            _stopButton.Click += (sender, args) => SendAudioCommand(new PlayingServiceInputModel() { Action = PlayingService.ActionStop, SeekBarPosition = 0 });

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

        //protected override void OnSaveInstanceState(Bundle outState)
        //{
        //    base.OnSaveInstanceState(outState);
        //    if (_show != null)
        //        outState.PutInt("ShowId", _show.Show.Id);
        //    if (_episode != null)
        //        outState.PutInt("EpisodeId", _episode.Episode.Id);
        //}

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
            string mediaStreamingTitle = _showTitle + ": " + _episode.Episode.EpisodeNumber + " - " + _episode.Episode.Label;
            intent.PutExtra("streamingDetails", mediaStreamingTitle);
            intent.PutExtra("EpisodeId", _episode.Episode.Id);
            intent.PutExtra("ShowTitle", _showTitle);
            intent.PutExtra("ShowId", _show.Show.Id);
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
            string saveKey = _show.Show.Id + "|" + _episode.Episode.Id + "|PlayBackPosition";
            var sharedPref = this.GetSharedPreferences(SAVE_FILE_NAME, FileCreationMode.Private);
            int defaultValue = result;

            result = sharedPref.GetInt(saveKey, defaultValue);

            if (result < 0) result = 0;

            return result;
        }

        private string GetTimeString(int time)
        {
            TimeSpan ts = TimeSpan.FromMilliseconds(time);
            string timeString = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    ts.Hours,
                                    ts.Minutes,
                                    ts.Seconds);
            return timeString;
        }
    
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}