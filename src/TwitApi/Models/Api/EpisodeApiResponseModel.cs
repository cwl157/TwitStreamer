using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwitApi.Models.Api
{
    internal class EpisodeApiResponseModel : IApiResponseModel
    {
        public int Id { get; set; }
        public string EpisodeNumber { get; set; }
        public string Label { get; set; }
        public string Teaser { get; set; }
        public DateTime AiringDate { get; set; }
        [JsonProperty("video_audio")]
        public VideoAudioDetailsApiResponseModel AudioDetails { get; set; }
    }
}
