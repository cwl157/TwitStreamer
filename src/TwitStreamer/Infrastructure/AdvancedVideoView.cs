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
using Android.Util;
using TwitStreamer.ViewModels;

namespace TwitStreamer.Infrastructure
{
    public class AdvancedVideoView : VideoView
    {
        // private ShowViewModel _show;
        //private EpisodeViewModel _episode;

        private const string SAVE_FILE_NAME = "PlayBackData";

        public AdvancedVideoView(Context context) : base(context)
        {
            
        }

        public AdvancedVideoView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            
        }

        public AdvancedVideoView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            
        }

        public override void Pause()
        {
            base.Pause();
            // Save current position here
            //Android.Widget.Toast.MakeText(this.Context, "Pause clicked", Android.Widget.ToastLength.Short).Show();
            SavePlayBackPosition();
        }

        public override void Start()
        {
            base.Start();
            //Android.Widget.Toast.MakeText(this.Context, "Start clicked", Android.Widget.ToastLength.Short).Show();
            // Load current position here
       //     GetPlayBackPosition();

        }

        private void SavePlayBackPosition()
        {
            ShowViewModel show = TwitApi.Instance.Shows.FirstOrDefault(s => s.Selected == true);
            if (show != null)
            {
                EpisodeViewModel episode = show.Episodes.FirstOrDefault(e => e.Selected == true);
                if (episode != null)
                {
                    // If we find the show and episode, save the playback position. If they are not found, it will not save the progress.
                    var sharedPref = Context.GetSharedPreferences(SAVE_FILE_NAME, FileCreationMode.Private);
                    var editor = sharedPref.Edit();

                    string saveKey = show.Show.Id + "|" + episode.Episode.Id + "|PlayBackPosition";
                    Console.WriteLine("Current Position: " + this.CurrentPosition);
                    editor.PutInt(saveKey, this.CurrentPosition);
                    editor.Commit();
                }
            }
        }

       

    }
}