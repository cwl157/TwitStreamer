using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Models.Api;

namespace TwitApi.Parsers
{
    internal interface IParser
    {
        TApiResponseModel Parse<TApiResponseModel>(string s) where TApiResponseModel : IApiResponseModel;
    }
}
