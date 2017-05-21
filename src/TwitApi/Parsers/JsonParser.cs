using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Models.Api;

namespace TwitApi.Parsers
{
    internal class JsonParser : IParser
    {
        public TApiResponseModel Parse<TApiResponseModel>(string apiResponse) where TApiResponseModel : IApiResponseModel
        {
            var parsedResponse = JsonConvert.DeserializeObject<TApiResponseModel>(apiResponse);

            return parsedResponse;
        }
    }
}
