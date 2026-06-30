using System;
using System.ComponentModel.DataAnnotations;
namespace DEM__Diamond_Event_Management_.Models
{
    public class Organisation

    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Organisation name is required.")]

        [Display(Name = "Organisation Name")]

        public string Name { get; set; }

        [Required(ErrorMessage = "Website domain is required.")]

        [Display(Name = "Website Domain")]
        public string Domain { get; set; }

        [Display(Name = "Anyone with this domain can join")]
        public bool AllowDomainJoin { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
