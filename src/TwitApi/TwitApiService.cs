using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TwitApi.Methods;
using TwitApi.Models;

namespace TwitApi
{
    public class TwitApiService
    {
        //public TResponseModel CallApi(IApiMethod method) 
        //{
        //    TResponseModel instance = (TResponseModel)Activator.CreateInstance(typeof(TResponseModel));
        //    Task<TResponseModel> result = new Task<TResponseModel>(() => { TResponseModel r = (TResponseModel)instance.Mapper.Map(method.Execute().Result); return r; });
        //    result.Start();
        //    result.Wait();
        //    return result.Result;
        //}

        public async Task<TResponseModel> CallApi<TResponseModel>(IApiMethod method) where TResponseModel : IResponseModel
        {
            TResponseModel instance = (TResponseModel)Activator.CreateInstance(typeof(TResponseModel));
            var result = await method.Execute();
            return (TResponseModel)instance.Mapper.Map(result);
        }
    }
}
