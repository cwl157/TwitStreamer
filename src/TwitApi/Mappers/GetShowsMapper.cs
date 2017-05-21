using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Api.Models;
using TwitApi.Models;
using TwitApi.Models.Api;
using TwitApi.Parsers;

namespace TwitApi.Mappers
{
    public class GetShowsMapper : IMapper
    {
        public IResponseModel Map(string apiResponse)
        {
            IParser jsonParser = new JsonParser();
            IApiResponseModel parsedResponse = jsonParser.Parse<ShowsApiResponseModel>(apiResponse);
            ShowsResponseModel responseModel = new ShowsResponseModel();
            ShowsApiResponseModel input = (ShowsApiResponseModel)parsedResponse;

            foreach (ShowApiResponseModel m in input.Shows)
            {
                responseModel.Shows.Add(new ShowResponseModel
                {
                    Id = m.Id,
                    Description = m.Description,
                    Label = m.Label
                });
            }

            return (IResponseModel)responseModel;
            
        }
    }
}
