using BlazorChat.Shared;
using System.Threading.Tasks;

namespace BlazorChat.Client.Managers
{
    public interface IUserManager
    {
        Task<ApiResponseModel> Signup(ApplicationUserRequest applicationUser);

        Task<ApiResponseModel> Login(LoginDTO login);

        Task<ApiResponseModel> Accounts();

        Task<ApiResponseModel> UserInfo(string id);

        Task<ApiResponseModel> UpdateTheme(string id, bool value);
    }
}
