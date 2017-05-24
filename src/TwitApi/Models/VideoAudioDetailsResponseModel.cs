using System;
using System.Collections.Generic;
using System.Text;

namespace TwitApi.Models
{
    public enum VideoAudioType
    {
        None,
        Audio,
        HDVideo,
        LargeVideo,
        SmallVideo
    }
    public class VideoAudioDetailsResponseModel
    {
        public VideoAudioType Type { get; set; }
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
