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

namespace TwitStreamer.ViewModels
{
    public enum SourceUI
    {
        None,
        PlayingService,
        PlayBackActivity
    }

    public class PlayingEpisodeViewModel
    {
        public bool IsPaused { get; set; }
        public string MediaUri { get; set; }
        public string StreamingDetails { get; set; }
        public int ShowId { get; set; }
        public string ShowTitle { get; set; }
        public int EpisodeId { get; set; }
        public int SavedPosition { get; set; }
        public int SeekBarPosition { get; set; }
        public bool IsPlaying { get; set; }
        public int MediaDuration { get; set; }
        public int MediaCurrentPosition { get; set; }
        public SourceUI Source { get; set; }

        public PlayingEpisodeViewModel()
        {
            Source = SourceUI.None;
        }
    }
}