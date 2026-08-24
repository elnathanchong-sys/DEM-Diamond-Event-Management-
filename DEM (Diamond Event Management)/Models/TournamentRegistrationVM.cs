using System.ComponentModel.DataAnnotations;

namespace DEM__Diamond_Event_Management_.Models;

public class TournamentRegistrationVM
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select a tournament.")]
    [Display(Name = "Tournament")]
    public int TournamentId { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Team name")]
    public string TeamName { get; set; } = string.Empty;

    [MinLength(1, ErrorMessage = "Add at least one player.")]
    public List<string> PlayerNames { get; set; } = [string.Empty];
}
