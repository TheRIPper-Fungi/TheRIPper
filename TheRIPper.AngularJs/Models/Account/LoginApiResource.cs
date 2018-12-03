using System.ComponentModel.DataAnnotations;

namespace TheRIPPer.Razor.Models.Account
{
    public class LoginApiResource
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}