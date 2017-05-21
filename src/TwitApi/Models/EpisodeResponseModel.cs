using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Mappers;

namespace TwitApi.Models
{
    public class EpisodeResponseModel
    {
        public int Id { get; set; }
        public string EpisodeNumber { get; set; }
        public string Label { get; set; }
        public string Teaser { get; set; }
        public DateTime AiringDate { get; set; }

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
