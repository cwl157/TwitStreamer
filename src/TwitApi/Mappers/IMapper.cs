using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Models;
using TwitApi.Models.Api;

namespace TwitApi.Mappers
{
    public interface IMapper
    {
        IResponseModel Map(string apiResponse);
    }
}
