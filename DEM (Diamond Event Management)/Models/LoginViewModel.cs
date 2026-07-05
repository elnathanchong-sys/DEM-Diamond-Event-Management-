using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

#nullable disable warnings

namespace DEM__Diamond_Event_Management_.Models
{
    public class LoginViewModel
    {
        [StringLength(100)]
        //[Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [StringLength(100, MinimumLength =5)]
        //[Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class CreateAccountViewModel
    {
        [StringLength(100)]
        [EmailAddress]
        [Remote("CheckEmail", "Account", ErrorMessage = "Duplicated {0}.")]
        public string Email { get; set; }

        [StringLength(100, MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [StringLength(100, MinimumLength = 5)]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        public string Confirm { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public IFormFile Photo { get; set; }
    }
}
