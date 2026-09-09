using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEM__Diamond_Event_Management_.Models
{
    public class CreateAccountVM
    {
        [StringLength(100)]
        [EmailAddress]
        [Remote("CheckEmail", "Account", ErrorMessage = "Duplicated {0}.")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 5)]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string Confirm { get; set; } = string.Empty;

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public IFormFile? Photo { get; set; }
    }

}