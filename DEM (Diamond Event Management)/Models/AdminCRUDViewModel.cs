using System.ComponentModel.DataAnnotations;

namespace DEM__Diamond_Event_Management_.Models
{
    public class AdminCreateViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(1)] // Adheres to MaxLength(1) on User
        public string Gender { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(100)]
        public string? Phone { get; set; }
    }

    public class AdminEditViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(1)] // Adheres to MaxLength(1) on User[cite: 7]
        public string Gender { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(100)]
        public string? Phone { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters.")]
        public string? NewPassword { get; set; }
    }
}