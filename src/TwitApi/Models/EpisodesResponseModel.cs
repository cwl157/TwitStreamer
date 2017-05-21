using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Mappers;

namespace TwitApi.Models
{
    public class EpisodesResponseModel : IResponseModel
    {
        public EpisodesResponseModel()
        {
            Episodes = new List<EpisodeResponseModel>();
        }
        public int Count { get; set; }
        public List<EpisodeResponseModel> Episodes { get; private set; }
        public IMapper Mapper { get { return new GetEpisodesMapper(); } }
    }
}
