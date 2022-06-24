using BlazorChat.Server.Service;
using BlazorChat.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BlazorChat.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserManager _userManager;
        public UserController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(ApplicationUserRequest applicationUser)
        {
            var response = await _userManager.Signup(applicationUser);

            var apiResponse = new ApiResponseModel(response.HasValidationError ?
                System.Net.HttpStatusCode.Conflict : System.Net.HttpStatusCode.OK, response.Message,
                response.Exception, response.Data);

            return Json(apiResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var response = await _userManager.Login(login);

            var apiResponse = new ApiResponseModel(response.HasValidationError ?
                System.Net.HttpStatusCode.Conflict : System.Net.HttpStatusCode.OK, response.Message,
                response.Exception, response.Data);

            return Json(apiResponse);
        }

        //[Authorize]
        [HttpGet("accounts")]
        public async Task<IActionResult> Accounts()
        {
            var response = await _userManager.Accounts();

            var apiResponse = new ApiResponseModel(response.HasValidationError ?
                System.Net.HttpStatusCode.Conflict : System.Net.HttpStatusCode.OK,
                response.Message, response.Exception, response.Data);

            return Json(apiResponse);
        }

        [Authorize]
        [HttpGet("userinfo/{id}")]
        public async Task<IActionResult> UserInfo(string id)
        {
            var response = await _userManager.UserInfo(id);

            var apiResponse = new ApiResponseModel(response.HasValidationError ?
                System.Net.HttpStatusCode.Conflict : System.Net.HttpStatusCode.OK, response.Message,
                response.Exception, response.Data);

            return Json(apiResponse);
        }

        [HttpGet("update-theme/{id}")]
        public async Task<IActionResult> UpdateTheme(string id, bool value)
        {
            var response = await _userManager.UpdateTheme(id, value);

            var apiResponse = new ApiResponseModel(response.HasValidationError ?
                System.Net.HttpStatusCode.Conflict : System.Net.HttpStatusCode.OK, response.Message,
                response.Exception, response.Data);

            return Json(apiResponse);
        }
    }
}
