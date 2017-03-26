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
using Android.Net.Wifi;
using Android.Net;
using TwitStreamer.Repositories;

namespace TwitStreamer
{
    [Service]
    [IntentFilter(new[] { ActionPlay, ActionPause, ActionStop })]
    public class PlayingService : Service, AudioManager.IOnAudioFocusChangeListener
    {
        private const string SAVE_FILE_NAME = "PlayBackData";
        //Actions
        public const string ActionPlay = "com.twitstreamer.action.PLAY";
        public const string ActionPause = "com.twitstreamer.action.PAUSE";
        public const string ActionStop = "com.twitstreamer.action.STOP";

        private MediaPlayer _player;
        private AudioManager _audioManager;
        private WifiManager _wifiManager;
        private WifiManager.WifiLock _wifiLock;
        private bool _paused;
        private string _mediaUri;
        private string _streamingDetails;
        private int _showId;
        private string _showTitle;
        private int _episodeId;
        private int _savedPosition;
        private int _seekBarPosition;

        public MediaPlayer Player
        {
            get { return _player; }
        }

        public bool Paused
        {
            get { return _paused; }
        }

        public int EpisodeId
        {
            get { return _episodeId; }
        }

        public int ShowId
        {
            get { return _showId; }
        }

        private const int NotificationId = 1;

        public static PlayingService ServiceInstance;

        //private const string Mp3 = @"http://www.podtrac.com/pts/redirect.mp3/cdn.twit.tv/audio/aaa/aaa0262/aaa0262.mp3";

        /// <summary>
        /// On create simply detect some of our managers
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            //Find our audio and notificaton managers
            _audioManager = (AudioManager)GetSystemService(AudioService);
            _wifiManager = (WifiManager)GetSystemService(WifiService);
            ServiceInstance = this;
        }

        /// <summary>
        /// Don't do anything on bind
        /// </summary>
        /// <param name="intent"></param>
        /// <returns></returns>
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public int CurrentlyPlayingEpisodeId
        {
            get { return _episodeId; }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            Console.WriteLine("In Service: Begin OnStartCommand");
            _mediaUri = intent.GetStringExtra("mediaUri");
            _streamingDetails = intent.GetStringExtra("streamingDetails");
            _showTitle = intent.GetStringExtra("ShowTitle");
            _episodeId = intent.GetIntExtra("EpisodeId", -1);
            _showId = intent.GetIntExtra("ShowId", -1);

            //_savedPosition = GetPlayBackPosition();
            _savedPosition = intent.GetIntExtra("SeekBarPosition", 0);
            Console.WriteLine("In Service: Get saved position "+_savedPosition);


            // var preferences = GetSharedPreferences(SAVE_FILE_NAME, FileCreationMode.Private);
            //   _savedPosition = preferences.GetInt("PlayBackPosition", 0);
            //   if (_savedPosition < 0) _savedPosition = 0;

            switch (intent.Action)
            {
                case ActionPlay: Play(); break;
                case ActionStop: Stop(); break;
                case ActionPause: Pause(); break;
            }

            //Set sticky as we are a long running operation
            return StartCommandResult.Sticky;
        }

        private void IntializePlayer()
        {
            _player = new MediaPlayer();

            //Tell our player to sream music
            _player.SetAudioStreamType(Stream.Music);

            //   _player.SeekTo(_savedPosition);

            //Wake mode will be partial to keep the CPU still running under lock screen
            _player.SetWakeMode(ApplicationContext, WakeLockFlags.Partial);

            //    forwardSong();

            //When we have prepared the song start playback
            _player.Prepared += (sender, args) => PlayerStart();

            //When we have reached the end of the song stop ourselves, however you could signal next track here.
            _player.Completion += (sender, args) => Stop();

            _player.Error += (sender, args) =>
            {
                //playback error
                Console.WriteLine("Error in playback resetting: " + args.What);
                Stop();//this will clean up and reset properly.
            };
        }

        private void PlayerStart()
        {
            Console.WriteLine("In Playing Service: Begin PlayerStart");
            forwardSong();
            _player.Start();

        }

        private async void Play()
        {

            if (_paused && _player != null)
            {
                _paused = false;

                //We are simply paused so just start again
                _player.Start();
                StartForeground();
                return;
            }

            if (_player == null)
            {
                IntializePlayer();
            }

            if (_player.IsPlaying)
                return;

            try
            {
                await _player.SetDataSourceAsync(ApplicationContext, Android.Net.Uri.Parse(_mediaUri));

                var focusResult = _audioManager.RequestAudioFocus(this, Stream.Music, AudioFocus.Gain);
                if (focusResult != AudioFocusRequest.Granted)
                {
                    //could not get audio focus
                    Console.WriteLine("Could not get audio focus");
                }
                _player.PrepareAsync();
                AquireWifiLock();
                StartForeground();
            }
            catch (Exception ex)
            {
                //unable to start playback log error
                Console.WriteLine("Unable to start playback: " + ex);
            }
        }

        /// <summary>
        /// When we start on the foreground we will present a notification to the user
        /// When they press the notification it will take them to the main page so they can control the music
        /// </summary>
        private void StartForeground()
        {
            Console.WriteLine("Start StartForeground method");
             Intent intent = new Intent(ApplicationContext, typeof(PlayBackActivity));
           
            intent.PutExtra("ShowTitle", _showTitle);
            intent.PutExtra("EpisodeId", _episodeId);
            //intent.PutExtra("MediaDuration", _player.Duration);
            //intent.PutExtra("MediaCurrentSpot", _player.CurrentPosition);
            Console.WriteLine("StarForeground method all PutExtras just happened");
            
            var pendingIntent = PendingIntent.GetActivity(ApplicationContext, 0,
                            intent,
                            PendingIntentFlags.UpdateCurrent);

            var notification = new Notification
            {
                TickerText = new Java.Lang.String(_streamingDetails+" started!"),
                Icon = Resource.Drawable.ic_stat_av_play_over_video
            };
            notification.Flags |= NotificationFlags.OngoingEvent;
            notification.SetLatestEventInfo(ApplicationContext, "TwitStreamer",
                            _streamingDetails, pendingIntent);
            StartForeground(NotificationId, notification);
        }

        private void Pause()
        {
            if (_player == null)
                return;

            if (_player.IsPlaying)
            {
                SavePlayBackPosition();
                _paused = true;
                _player.Pause();
            }

            StopForeground(true);
            //_paused = true;
           
        }

        public void Stop()
        {
            if (_player == null)
                return;

           

            if (_player.IsPlaying)
            {
                SavePlayBackPosition();
                _player.Stop();
            }

            _player.Reset();
            _paused = false;
            StopForeground(true);
            ReleaseWifiLock();
        }

        /// <summary>
        /// Lock the wifi so we can still stream under lock screen
        /// </summary>
        private void AquireWifiLock()
        {
            if (_wifiLock == null)
            {
                _wifiLock = _wifiManager.CreateWifiLock(WifiMode.Full, "xamarin_wifi_lock");
            }
            _wifiLock.Acquire();
        }

        /// <summary>
        /// This will release the wifi lock if it is no longer needed
        /// </summary>
        private void ReleaseWifiLock()
        {
            if (_wifiLock == null)
                return;

            _wifiLock.Release();
            _wifiLock = null;
        }

        /// <summary>
        /// Properly cleanup of your player by releasing resources
        /// </summary>
        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_player != null)
            {
                SavePlayBackPosition();
           //     var preferences = GetSharedPreferences(SAVE_FILE_NAME, FileCreationMode.Private);
           //     var editor = preferences.Edit();
           //    editor.PutInt("PlayBackPosition", _player.CurrentPosition);
           //   editor.Commit();
                _player.Release();
                _player = null;
            }
        }

        /// <summary>
        /// For a good user experience we should account for when audio focus has changed.
        /// There is only 1 audio output there may be several media services trying to use it so
        /// we should act correctly based on this.  "duck" to be quiet and when we gain go full.
        /// All applications are encouraged to follow this, but are not enforced.
        /// </summary>
        /// <param name="focusChange"></param>
        public void OnAudioFocusChange(AudioFocus focusChange)
        {
            switch (focusChange)
            {
                case AudioFocus.Gain:
                    if (_player == null)
                        IntializePlayer();

                    if (!_player.IsPlaying)
                    {
                        _player.Start();
                        _paused = false;
                    }

                    _player.SetVolume(1.0f, 1.0f);//Turn it up!
                    break;
                case AudioFocus.Loss:
                    //We have lost focus stop!
                    Stop();
                    break;
                case AudioFocus.LossTransient:
                    //We have lost focus for a short time, but likely to resume so pause
                    Pause();
                    break;
                case AudioFocus.LossTransientCanDuck:
                    //We have lost focus but should till play at a muted 10% volume
                    if (_player.IsPlaying)
                        _player.SetVolume(.1f, .1f);//turn it down!
                    break;

            }
        }

        private void SavePlayBackPosition()
        {
            var sharedPref = GetSharedPreferences(SAVE_FILE_NAME, FileCreationMode.Private);
            var editor = sharedPref.Edit();
            //var show = TwitRepository.Shows.FirstOrDefault(s => s.Label == _showTitle);
           // var episode = TwitRepository.Episodes.FirstOrDefault(e => e.Id == _episodeId);
          
            // Test saving with just _showId and _episodeId, don't find them
            string saveKey = _showId + "|" + _episodeId+"|PlayBackPosition";
            Console.WriteLine("Current Position: "+_player.CurrentPosition);
            editor.PutInt(saveKey, _player.CurrentPosition);
            editor.Commit();
        }

     /*   private int GetPlayBackPosition()
        {
            int result = 0;
            var show = TwitRepository.Shows.FirstOrDefault(s => s.Label == _showTitle);
            var episode = TwitRepository.Episodes.FirstOrDefault(e => e.Id == _episodeId);
            string saveKey = show.Id + "|" + episode.Id+"|PlayBackPosition";
            var sharedPref = this.GetSharedPreferences(SAVE_FILE_NAME, FileCreationMode.Private);
            int defaultValue = result;

            result = sharedPref.GetInt(saveKey, defaultValue);

            if (result < 0) result = 0;


            return result;
        }
        */
        private void forwardSong()
        {
            if (_player != null)
            {
                int currentPosition = _player.CurrentPosition;
                if (currentPosition + _savedPosition <= _player.Duration)
                {
                    _player.SeekTo(currentPosition + _savedPosition);
                }
                else
                {
                    _player.SeekTo(_player.Duration);
                }
            }
        }

    }

}