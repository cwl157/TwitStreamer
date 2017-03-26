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
using Newtonsoft.Json;

namespace TwitStreamer.Models
{
    public class EpisodeResponseModel
    {
        public int Id { get; set; }
        public string EpisodeNumber { get; set; }
        public string Label { get; set; }
        public string Teaser { get; set; }
        public DateTime AiringDate { get; set; }
        [JsonProperty("video_audio")]
        public VideoAudioDetailsResponseModel AudioDetails { get; set; }
    }
}