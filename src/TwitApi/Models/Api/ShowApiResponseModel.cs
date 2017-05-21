using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Models.Api;

namespace TwitApi.Api.Models
{
    internal class ShowApiResponseModel : IApiResponseModel
    {
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
    }
}
