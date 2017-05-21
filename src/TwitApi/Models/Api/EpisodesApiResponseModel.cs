using System;
using System.Collections.Generic;
using System.Text;

namespace TwitApi.Models.Api
{
    internal class EpisodesApiResponseModel : IApiResponseModel
    {
        public int Count { get; set; }
        public List<EpisodeApiResponseModel> Episodes { get; set; }
    }
}
