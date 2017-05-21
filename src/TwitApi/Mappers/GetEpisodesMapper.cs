using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Models;
using TwitApi.Models.Api;
using TwitApi.Parsers;

namespace TwitApi.Mappers
{
    public class GetEpisodesMapper : IMapper
    {
        public IResponseModel Map(string apiResponse)
        {
            IParser jsonParser = new JsonParser();
            IApiResponseModel parsedResponse = jsonParser.Parse<EpisodesApiResponseModel>(apiResponse);
            EpisodesResponseModel responseModel = new EpisodesResponseModel();
            EpisodesApiResponseModel input = (EpisodesApiResponseModel)parsedResponse;

            foreach (EpisodeApiResponseModel m in input.Episodes)
            {
                responseModel.Episodes.Add(new EpisodeResponseModel
                {
                    Id = m.Id,
                    EpisodeNumber = m.EpisodeNumber,
                    Label = m.Label,
                    Teaser = m.Teaser,
                    AiringDate = m.AiringDate,

                    MediaUrl = m.AudioDetails.MediaUrl,
                    Format = m.AudioDetails.Format,
                    Guid = m.AudioDetails.Guid,
                    Size = m.AudioDetails.Size,
                    Changed = m.AudioDetails.Changed,
                    RunningTime = m.AudioDetails.RunningTime,
                    Hours = m.AudioDetails.Hours,
                    Minutes = m.AudioDetails.Minutes,
                    Seconds = m.AudioDetails.Seconds
                });
            }

            return (IResponseModel)responseModel;
        }
    }
}
