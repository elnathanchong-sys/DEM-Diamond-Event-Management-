using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable warnings

namespace DEM__Diamond_Event_Management_.Models
{
    public class RegistrationVM
    {
        [Required]
        public string EventId { get; set; }

        [Required(ErrorMessage = "Please enter your team name.")]
        [StringLength(100)]
        public string TeamName { get; set; }

        public List<string> PlayerIGNs { get; set; } = [];
    }
}
