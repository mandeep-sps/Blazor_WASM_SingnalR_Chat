using BlazorChat.Shared;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorChat.Client.Managers
{
    public class UserManager : IUserManager
    {
        private readonly HttpClient httpClient;
        private readonly string apiurl;

        public UserManager(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            apiurl = "api/user/";
        }
        public async Task<ApiResponseModel> Accounts()
        {
            return await ResponseHandler.GetApiResponse(await httpClient.GetAsync($"api/user/accounts"));
        }

        public async Task<ApiResponseModel> Login(LoginDTO login)
        {
            return await ResponseHandler.GetApiResponse(await httpClient.PostAsJsonAsync(apiurl + "login", login));
        }

        public async Task<ApiResponseModel> Signup(ApplicationUserRequest applicationUser)
        {
            return await ResponseHandler.GetApiResponse
                (await httpClient.PostAsJsonAsync(apiurl + "signup", applicationUser));

        }

        public async Task<ApiResponseModel> UserInfo(string id)
        {
            return await ResponseHandler.GetApiResponse
                 (await httpClient.GetAsync($"api/user/userinfo/" + id));
        }

        public async Task<ApiResponseModel> UpdateTheme(string id, bool value)
        {
            return await ResponseHandler.GetApiResponse
                 (await httpClient.GetAsync($"api/user/update-theme/" + id + "?value=" + value));
        }
    }
}
