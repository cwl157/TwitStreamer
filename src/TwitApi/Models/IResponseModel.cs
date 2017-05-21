using System;
using System.Collections.Generic;
using System.Text;
using TwitApi.Mappers;

namespace TwitApi.Models
{
    public interface IResponseModel
    {
        IMapper Mapper { get; }
    }
}
