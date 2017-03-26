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

namespace TwitStreamer.Models
{
    public class VideoAudioDetailsResponseModel
    {
        public string MediaUrl { get; set; }
        public string Format { get; set; }
        public string Guid { get; set; }
        public string Size { get; set; }
        public string Changed { get; set; }
        public TimeSpan RunningTime { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
}