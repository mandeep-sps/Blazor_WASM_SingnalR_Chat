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

        public Task<ApiResponseModel> UserInfo(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
