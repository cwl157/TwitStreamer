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
using TwitStreamer.Infrastructure;

namespace TwitStreamer
{
    [Activity(Label = "VideoViewerActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape, ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class VideoViewerActivity : Activity
    {
        private const string SAVE_FILE_NAME = "PlayBackData";

        private ShowViewModel _show;
        private EpisodeViewModel _episode;
        private MediaController _mediaController;
        private AdvancedVideoView _videoPlayer;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.VideoPlayBack);

            _show = TwitApi.Instance.Shows.FirstOrDefault(s => s.Selected == true);
            if (_show == null)
            {
                // return to MainActivity
            }

            _episode = _show.Episodes.FirstOrDefault(e => e.Selected == true);
            if (_episode == null)
            {
                // return to MainActivity
            }

            
            _videoPlayer = FindViewById<AdvancedVideoView>(Resource.Id.videoViewer);
            _mediaController = new MediaController(this, true);
           
            _videoPlayer.SetMediaController(_mediaController);
            string mediaUrl = Intent.GetStringExtra("VideoUrl");

            _videoPlayer.SetVideoPath(mediaUrl);

            // Create your application here
        }

        protected override void OnRestart()
        {
            base.OnRestart();
        }

        protected override void OnStart()
        {
            base.OnStart();
            _videoPlayer.Prepared += OnVideoPlayerPrepared;
        }

        protected override void OnStop()
        {
                 _videoPlayer.Prepared -= OnVideoPlayerPrepared;
            //    _mediaController.Dispose();
            //    _videoPlayer.Dispose();
            // TODO: Possibly save position here
            _videoPlayer.StopPlayback();
            base.OnStop();
        }

        protected override void OnDestroy()
        {
            _mediaController.Dispose();
            _videoPlayer.Dispose();
            base.OnDestroy();
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            //ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            //newConfig.Orientation = Android.Content.Res.Orientation.Landscape;
            // Do something to _videoPlayer so prepared gets called
            // Super hacky. The only time OnCofigurationChanged is called is when coming and going from the lock screen
            // so when going to the lock screen back out to the EpisodeDetailsActivity.
            // This will launch the EpisodeDetailsActivity just to hide it which is a huge waste of resources (I think)
            // But then coming back from the lock screen, the episode details activity will show and then the user can pick to continue watching the episode.
            Intent i = new Intent(this.ApplicationContext, typeof(EpisodeDetailsActivity));
            StartActivity(i);
        }

        private void OnVideoPlayerPrepared(object sender, EventArgs e)
        {
            _mediaController.SetAnchorView(_videoPlayer);
            //show media controls for 3 seconds when video starts to play
            _mediaController.Show(3000);
            int loadedPosition = GetPlayBackPosition();
            if (loadedPosition > 0)
            {
                _videoPlayer.SeekTo(loadedPosition);
            }
            _videoPlayer.Start();
        }

        private int GetPlayBackPosition()
        {
            int result = 0;
            ShowViewModel show = TwitApi.Instance.Shows.FirstOrDefault(s => s.Selected == true);
            if (show != null)
            {
                EpisodeViewModel episode = show.Episodes.FirstOrDefault(e => e.Selected == true);
                if (episode != null)
                {
                    // If we find a show and episode, get the playback position. If not it will just start from the beginning.
                    string saveKey = show.Show.Id + "|" + episode.Episode.Id + "|PlayBackPosition";
                    var sharedPref = this.GetSharedPreferences(SAVE_FILE_NAME, FileCreationMode.Private);
                    int defaultValue = result;

                    result = sharedPref.GetInt(saveKey, defaultValue);

                    if (result < 0) result = 0;
                }
            }
            return result;
        }
    }
}