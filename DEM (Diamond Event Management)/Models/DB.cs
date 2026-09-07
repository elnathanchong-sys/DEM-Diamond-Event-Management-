using DEM__Diamond_Event_Management_.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace DEM__Diamond_Event_Management_.Models;

#nullable disable warnings

public class DB(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> User{ get; set; }
    public DbSet<Member> Member { get; set; }
    public DbSet<Admin> Admin { get; set; }
    public DbSet<Event> Event { get; set; }
    public DbSet<IndexBanner> IndexBanner { get; set; }
    public DbSet<Registration> Registration { get; set; }
    public DbSet<Player> Player { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<Team> Team { get; set; }
    public DbSet<Tournament> Tournament { get; set; }
    public DbSet<TournamentRegistration> TournamentRegistration { get; set; }
    public DbSet<Venue> Venue { get; set; }
}

public class User
{
    // Column
    [Key, MaxLength(100)]
    public string Email { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string HashedPassword { get; set; }
    [MaxLength(100)]
    public string Phone { get; set; }
    [MaxLength(1)]
    public string Gender { get; set; }
    [MaxLength(100)]
    public string Role => GetType().Name;
    [MaxLength(100)]
    public string UserImg { get; set; }
    [Column(TypeName = "DATE")]
    public DateOnly CreationDate { get; set; }
    [MaxLength(100)]
    public string OngoingEvent { get; set; }
    [MaxLength(100)]
    public string UpcomingEvent { get; set; }

    // Navigation
    public List<Registration> Registrations { get; set; } = [];
}

public class Member : User
{
    [MaxLength(100)]
    public string Country { get; set; }

    [MaxLength(100)]
    public string State { get; set; }

    [MaxLength(100)]
    public string City { get; set; }

    [MaxLength(100)]
    public string HouseNo { get; set; }

    [MaxLength(100)]
    public string Address { get; set; }

    // Membership information
    [MaxLength(20)]
    public string MembershipLevel { get; set; } = "Silver";

    public int RewardPoints { get; set; } = 0;
}

public class Admin : User
{

}

public class Event
{
    // Column testing one
    [Key, MaxLength(6)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string Description { get; set; }
    [Column(TypeName = "DATE")]
    public DateOnly StartDate { get; set; }
    [Column(TypeName = "DATE")]
    public DateOnly EndDate { get; set; }
    [Column(TypeName = "TIME")]
    public TimeOnly StartTime { get; set; }
    [Column(TypeName = "TIME")]
    public TimeOnly EndTime { get; set; }
    [Precision(6, 2)]
    public decimal Price { get; set; }
    [MaxLength(100)]
    public string PosterImg { get; set; }
    [MaxLength(100)]
    public string BannerImg { get; set; }
    [MaxLength(10)]
    public string Status { get; set; }
    [Range(0, 1000)]
    public int TotalTickets { get; set; }
    [Range(0, 1000)]
    public int TicketSold { get; set; }
    [Range(1, 20, ErrorMessage = "Max players per team must be between 1 and 20")]
    public int MaxPlayersPerTeam { get; set; } = 7;
    [Range(1, 1000)]
    public int MaxTeams { get; set; }
    [Precision(10, 2)]
    public decimal PrizePool { get; set; }



    //Foreign Key
    public String CategoryId { get; set; }
    public String VenueId { get; set; }
    // Navigation
    public Category Category { get; set; }
    public Venue Venue { get; set; }
    public List<Registration> Registrations { get; set; } = [];
}

public class IndexBanner
{
    [Key, MaxLength(6)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string IndexBannerImg { get; set; }
}

public class Registration
{
    // Column
    [Key, MaxLength(6)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string TeamName { get; set; }
    [Column(TypeName = "DATE")]
    public DateOnly RegDate { get; set; }

    [MaxLength(20)]
    public string Status { get; set; }

    // Foreign Keys
    public string UserEmail { get; set; }
    public string EventId { get; set; }
    // Navigation
    public User User { get; set; }
    public Event Event { get; set; }
    public List<Player> Players { get; set; } = [];
}

public class Player
{
    [Key, MaxLength(6)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string IGN { get; set; }
    [MaxLength(20)]
    public string Role { get; set; } = "Player"; //Default value is Player

    // Foreign Key
    public string TeamRegistrationId { get; set; }
    // Navigation
    public Registration Registration { get; set; }
}

public class Category
{
    // Column
    [Key, MaxLength(4)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string CategoryName { get; set; }

    // Navigation
    public List<Event> Events { get; set; } = [];
}

public class Venue
{
    [Key, MaxLength(4)]
    public string Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string VenueName { get; set; }

    [Required]
    [MaxLength(200)]
    public string Location { get; set; }

    [Range(1, 100000, ErrorMessage = "Capacity must be between 1 and 100,000")]
    public int Capacity { get; set; }

    public string? Description { get; set; }

    public List<Event> Events { get; set; } = [];

    [Range(1, 100000)]
    public int VenueCapacity { get; set; }
}

public class Payment
{
    [Key, MaxLength(4)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string PaymentMethod { get; set; }
    [Precision (10, 2)]
    public decimal AmountPaid { get; set; }
    [Column(TypeName = "DATE")]
    public DateOnly PaymentDate { get; set; }

    public string RegistrationId { get; set; }
    public Registration Registration { get; set; }

}

//Ho's newly added table is causing issue for the exisitng player table which was previosuly added 
//public class Player
//{
//    public int Id { get; set; }
//
//    [Required, StringLength(100)]
//    public string FullName { get; set; } = string.Empty;

//    [Required]
//    public int TeamId { get; set; }

//    public Team Team { get; set; } = null!;
//}


public enum RegistrationStatus
{
    Pending,
    Approved,
    Rejected
}

public class Team
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    // Email of the authenticated member who created this team.
    [Required, StringLength(100)]
    public string CaptainEmail { get; set; } = string.Empty;

    public List<Player> Players { get; set; } = [];
    public List<TournamentRegistration> Registrations { get; set; } = [];
}

public class Tournament
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Name { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateOnly RegistrationDeadline { get; set; }

    [Range(1, 1000)]
    public int MaxTeams { get; set; }

    [Range(1, 100)]
    public int MinPlayersPerTeam { get; set; }

    [Range(1, 100)]
    public int MaxPlayersPerTeam { get; set; }

    public List<TournamentRegistration> Registrations { get; set; } = [];
}

public class TournamentRegistration
{
    public int Id { get; set; }

    [Required]
    public int TournamentId { get; set; }
    public Tournament Tournament { get; set; } = null!;

    [Required]
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!;

    [Required, StringLength(100)]
    public string CaptainEmail { get; set; } = string.Empty;

    public RegistrationStatus Status { get; set; } = RegistrationStatus.Pending;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? AdminNote { get; set; }

    [StringLength(100)]
    public string? ReviewedByEmail { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
