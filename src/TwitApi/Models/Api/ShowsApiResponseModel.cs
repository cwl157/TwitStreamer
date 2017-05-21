using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Api.Models;

namespace TwitApi.Models.Api
{
    internal class ShowsApiResponseModel : IApiResponseModel
    {
        public int Count { get; set; }

        public List<ShowApiResponseModel> Shows { get; set; }
    }
}
