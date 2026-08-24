using System.ComponentModel.DataAnnotations;

namespace DEM__Diamond_Event_Management_.Models
{
    public class ForgotPasswordViewModel
    {
        [StringLength(100)]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters.")]
        [Required(ErrorMessage = "New Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}