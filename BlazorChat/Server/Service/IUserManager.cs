using BlazorChat.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorChat.Server.Service
{
    public interface IUserManager
    {
        Task<ServiceResult<bool>> Signup(ApplicationUserRequest applicationUser);

        Task<ServiceResult<LoginResponse>> Login(LoginDTO login);

        Task<ServiceResult<List<ApplicationUserResponse>>> Accounts();

        Task<ServiceResult<ApplicationUserResponse>> UserInfo(string id);
    }
}
