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

public class EditProfileViewModel
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string HouseNo { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string UserImg { get; set; } = string.Empty;
}
