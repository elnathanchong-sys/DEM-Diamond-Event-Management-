using System.ComponentModel.DataAnnotations;

namespace DEM__Diamond_Event_Management_.Models
{
    public class AdminUserEditViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(1)]
        public string Gender { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        [StringLength(100)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? HouseNo { get; set; }

        [StringLength(100)]
        public string? Address { get; set; }

        [StringLength(100)]
        public string? MembershipLevel { get; set; } = "Silver";

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Password must be at least 5 characters.")]
        public string? Password { get; set; }

        public bool IsBlocked { get; set; }
    }
}
