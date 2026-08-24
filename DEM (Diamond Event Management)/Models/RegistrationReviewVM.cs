using System.ComponentModel.DataAnnotations;

namespace DEM__Diamond_Event_Management_.Models;

public class RegistrationReviewVM
{
    public int RegistrationId { get; set; }

    [StringLength(500)]
    [Display(Name = "Note for captain (optional)")]
    public string? AdminNote { get; set; }
}
