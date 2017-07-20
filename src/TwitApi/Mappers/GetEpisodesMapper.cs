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
                    AudioDetails = new VideoAudioDetailsResponseModel
                    {
                        Type = VideoAudioType.Audio,
                        MediaUrl = m.AudioDetails.MediaUrl,
                        Format = m.AudioDetails.Format,
                        Guid = m.AudioDetails.Guid,
                        Size = m.AudioDetails.Size,
                        Changed = m.AudioDetails.Changed,
                        RunningTime = m.AudioDetails.RunningTime,
                        Hours = m.AudioDetails.Hours,
                        Minutes = m.AudioDetails.Minutes,
                        Seconds = m.AudioDetails.Seconds
                    },
                    HdVideoDetails = new VideoAudioDetailsResponseModel
                    {
                        Type = VideoAudioType.HDVideo,
                        MediaUrl = m.HdVideoDetails.MediaUrl,
                        Format = m.HdVideoDetails.Format,
                        Guid = m.HdVideoDetails.Guid,
                        Size = m.HdVideoDetails.Size,
                        Changed = m.HdVideoDetails.Changed,
                        RunningTime = m.HdVideoDetails.RunningTime,
                        Hours = m.HdVideoDetails.Hours,
                        Minutes = m.HdVideoDetails.Minutes,
                        Seconds = m.HdVideoDetails.Seconds
                    },
                    LargeVideoDetails = new VideoAudioDetailsResponseModel
                    {
                        Type = VideoAudioType.LargeVideo,
                        MediaUrl = m.LargeVideoDetails.MediaUrl,
                        Format = m.LargeVideoDetails.Format,
                        Guid = m.LargeVideoDetails.Guid,
                        Size = m.LargeVideoDetails.Size,
                        Changed = m.LargeVideoDetails.Changed,
                        RunningTime = m.LargeVideoDetails.RunningTime,
                        Hours = m.LargeVideoDetails.Hours,
                        Minutes = m.LargeVideoDetails.Minutes,
                        Seconds = m.LargeVideoDetails.Seconds
                    },
                    SmallVideoDetails = new VideoAudioDetailsResponseModel
                    {
                        Type = VideoAudioType.SmallVideo,
                        MediaUrl = m.SmallVideoDetails.MediaUrl,
                        Format = m.SmallVideoDetails.Format,
                        Guid = m.SmallVideoDetails.Guid,
                        Size = m.SmallVideoDetails.Size,
                        Changed = m.SmallVideoDetails.Changed,
                        RunningTime = m.SmallVideoDetails.RunningTime,
                        Hours = m.SmallVideoDetails.Hours,
                        Minutes = m.SmallVideoDetails.Minutes,
                        Seconds = m.SmallVideoDetails.Seconds
                    }
                }); // End create new episode
            } // end foreach

            return (IResponseModel)responseModel;
        }
    }
}
