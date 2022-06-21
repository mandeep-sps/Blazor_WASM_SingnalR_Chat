using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorChat.Shared
{
    public class ApplicationUserResponse
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public DateTime AuditedOn { get; set; }
    }


    public class ApplicationUserRequest : ApplicationUserResponse
    {
        public string RepeatPassword { get; set; }
    }

    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Password must be at least 8 characters long.", MinimumLength = 8)]
        public string Password { get; set; }

    }

    public class LoginResponse : ApplicationUserResponse
    {
        public string Token { get; set; }
    }
}
