using System.ComponentModel.DataAnnotations;

namespace DEM__Diamond_Event_Management_.Models
{
    public class AdminCreateViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(1)] // Adheres to MaxLength(1) on User
        public string Gender { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(100)]
        public string? Phone { get; set; }
    }

    public class AdminEditViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression("^(M|F)$", ErrorMessage = "Please select a valid gender.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(100)]
        public string Phone { get; set; } = string.Empty;

        public string UserImg { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters.")]
        public string? NewPassword { get; set; }
    }
}