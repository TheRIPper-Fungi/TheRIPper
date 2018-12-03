using System.ComponentModel.DataAnnotations;

namespace TheRIPPer.Razor.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}