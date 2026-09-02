namespace DEM__Diamond_Event_Management_.Models;

public class ProfileViewModel
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string UserImg { get; set; }
    public DateOnly CreationDate { get; set; }

    public string MembershipLevel { get; set; }
    public int RewardPoints { get; set; }

    public List<ProfileEventViewModel> UpcomingEvents { get; set; } = [];
    public List<ProfilePurchaseViewModel> Purchases { get; set; } = [];
}

public class ProfileEventViewModel
{
    public string EventName { get; set; }
    public DateOnly StartDate { get; set; }
    public TimeOnly StartTime { get; set; }
    public string Venue { get; set; }
    public string PosterImg { get; set; }
    public string RegistrationId { get; set; }
}

public class ProfilePurchaseViewModel
{
    public string EventName { get; set; }
    public DateOnly RegistrationDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
}