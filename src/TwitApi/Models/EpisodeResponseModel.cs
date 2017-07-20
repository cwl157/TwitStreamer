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

        public VideoAudioDetailsResponseModel AudioDetails { get; set; }
        public VideoAudioDetailsResponseModel HdVideoDetails { get; set; }
        public VideoAudioDetailsResponseModel LargeVideoDetails { get; set; }
        public VideoAudioDetailsResponseModel SmallVideoDetails { get; set; }
    }
}
