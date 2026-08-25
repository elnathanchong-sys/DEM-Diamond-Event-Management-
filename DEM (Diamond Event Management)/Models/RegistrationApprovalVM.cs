using System.ComponentModel.DataAnnotations;

#nullable disable warnings

namespace DEM__Diamond_Event_Management_.Models
{
    //For RegistrationEvents UI similar of EventList 
    public class EventApprovalVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PosterImg { get; set; }
        public string Status { get; set; }
        public string CategoryId { get; set; }

        //How many teams for this event still need an admin decision
        public int PendingCount { get; set; }
    }

    //For RegistrationTeams page for UI similar of EventRead keeping it simple
    public class TeamRowVM
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public DateOnly RegDate { get; set; }
        public string Status { get; set; }
    }

    //For RegistrationDetails page to show team info and players ign and also to provide status updates
    public class TeamDetailVM
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public DateOnly RegDate { get; set; }
        public string Status { get; set; }
        public string UserEmail { get; set; }

        public string EventId { get; set; }
        public string EventName { get; set; }

        public List<PlayerRowVM> Players { get; set; } = [];
    }

    public class PlayerRowVM
    {
        public string IGN { get; set; }
        public string Role { get; set; }
    }
}
