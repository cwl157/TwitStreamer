using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Mappers;
using TwitApi.Methods;

namespace TwitApi.Models
{
    public class ShowsResponseModel : IResponseModel
    {
        public List<ShowResponseModel> Shows { get; private set; }

        public ShowsResponseModel()
        {
            Shows = new List<Models.ShowResponseModel>();
        }

        public IMapper Mapper
        {
            get { return new GetShowsMapper(); }
        }

        internal IApiMethod Method
        {
            get { return new GetShows(); }
        }
    }
}
