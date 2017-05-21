using System;
using System.Collections.Generic;
using System.Text;

namespace TwitApi.Models
{
    public class GetEpisodesRequestModel : IRequestModel
    {
        public int ShowId { get; set; }
    }
}
