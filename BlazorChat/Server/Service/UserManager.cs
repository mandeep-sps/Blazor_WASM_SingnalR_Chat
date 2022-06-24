using BlazorChat.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorChat.Server.Service
{
    public class UserManager : IUserManager
    {
        private readonly IRepository repository;
        private readonly IConfiguration configuration;

        public UserManager(IRepository repository, IConfiguration configuration)
        {
            this.repository = repository;
            this.configuration = configuration;
        }

        public async Task<ServiceResult<bool>> Signup(ApplicationUserRequest applicationUser)
        {
            try
            {
                if (await repository.FindBy<ApplicationUser>(x => x.Email == applicationUser.Email).AnyAsync())
                {
                    return new ServiceResult<bool>(false, "Email already exist", true);
                }

                ApplicationUser model = new()
                {
                    Name = applicationUser.Name,
                    Email = applicationUser.Email
                };

                model.Hash = Utils.CreateHash();
                model.Password = Utils.PasswordHash(applicationUser.Password, model.Hash);

                return new ServiceResult<bool>(await repository.AddAsync(model) > 0, "Account has been created");
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>(ex, ex.Message);
            }
        }


        public async Task<ServiceResult<LoginResponse>> Login(LoginDTO login)
        {
            try
            {
                var data = await repository.FindBy<ApplicationUser>(x => x.Email == login.Email).AsNoTracking().FirstOrDefaultAsync();

                if (data != null)
                {
                    var pwd = Utils.PasswordHash(login.Password, data.Hash);
                    if (data.Password.Equals(pwd))
                    {
                        LoginResponse response = new()
                        {
                            Id = data.Id,
                            Email = login.Email,
                            Name = data.Name,
                            Password = login.Password,
                            Token = GenerateJSONWebToken(data)
                        };

                        return new ServiceResult<LoginResponse>(response, "Logged in successfully");
                    }
                }

                return new ServiceResult<LoginResponse>(null, "Invalid credentials", true);
            }
            catch (Exception ex)
            {
                return new ServiceResult<LoginResponse>(ex, ex.Message);
            }
        }

        public async Task<ServiceResult<List<ApplicationUserResponse>>> Accounts()
        {
            try
            {
                var accounts = await repository.GetAll<ApplicationUser>().Select(UserExpression()).OrderBy(x => x.Name).ToListAsync();

                return new ServiceResult<List<ApplicationUserResponse>>(accounts, $"{accounts.Count} Accounts found");
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<ApplicationUserResponse>>(ex, ex.Message);
            }
        }

        public async Task<ServiceResult<ApplicationUserResponse>> UserInfo(string id)
        {
            try
            {
                var userInfo = await repository.FindBy<ApplicationUser>(x => x.Id == id).Select(UserExpression()).FirstOrDefaultAsync();

                return new ServiceResult<ApplicationUserResponse>(userInfo, $"User Info");
            }
            catch (Exception ex)
            {
                return new ServiceResult<ApplicationUserResponse>(ex, ex.Message);
            }
        }

        public async Task<ServiceResult<bool>> UpdateTheme(string id, bool value)
        {
            try
            {
                var userInfo = await repository.FindBy<ApplicationUser>(x => x.Id == id).FirstOrDefaultAsync();
                userInfo.IsDark = value;
                await repository.UpdateAsync(userInfo);
                return new ServiceResult<bool>(userInfo.IsDark, "Account has been created");
            }
            catch (Exception ex)
            {
                return new ServiceResult<bool>(ex, ex.Message);
            }
        }

        /// <summary>
        /// Private Expression for Code repete
        /// </summary>
        /// <returns></returns>
        private static Expression<Func<ApplicationUser, ApplicationUserResponse>> UserExpression()
        {
            return x => new ApplicationUserResponse
            {
                Email = x.Email,
                Id = x.Id,
                Name = x.Name,
                Password = x.Password,
                AuditedOn = x.AuditedOn,
                IsDark = x.IsDark
            };
        }

        private string GenerateJSONWebToken(ApplicationUser userInfo)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
                  {
                        new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", userInfo.Id.ToString()),
                        new Claim("Email", userInfo.Email),
                        new Claim("Name", userInfo.Name)
                    };
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"], configuration["Jwt:Issuer"],
                claims, expires: DateTime.Now.AddHours(2), signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
