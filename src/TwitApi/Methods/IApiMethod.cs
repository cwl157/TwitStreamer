using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitApi.Models.Api;

namespace TwitApi.Methods
{
    public interface IApiMethod
    {
        Task<string> Execute();
    }
}
