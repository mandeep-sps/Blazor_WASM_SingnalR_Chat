using BlazorChat.Shared;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BlazorChat.Client
{
    public static class ResponseHandler
    {
        public static async Task<ApiResponseModel> GetApiResponse(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<ApiResponseModel>(await response.Content.ReadAsStringAsync());
        }
    }
}
