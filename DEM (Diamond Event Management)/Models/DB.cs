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
    public DbSet<Registration> Registration { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Venue> Venue { get; set; }
    public DbSet<Payment> Payment { get; set; }
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
    [Precision(4, 2)]
    public decimal Price { get; set; }
    [MaxLength(100)]
    public string PosterImg { get; set; }
    [MaxLength(100)]
    public string BannerImg { get; set; }
    [MaxLength(10)]
    public string Status { get; set; }
    [MaxLength(100)]
    public int TotalTickets { get; set; }
    [MaxLength(100)]
    public int TicketSold { get; set; }


    //Foreign Key
    public String CategoryId { get; set; }
    public String VenueId { get; set; }
    // Navigation
    public Category Category { get; set; }
    public Venue Venue { get; set; }
    public List<Registration> Registrations { get; set; } = [];
}

public class Registration
{
    // Column
    [Key, MaxLength(4)]
    public string Id { get; set; }
    [Column(TypeName = "DATE")]
    public DateOnly RegDate { get; set; }
    [MaxLength(20)]
    public string Status { get; set; }
    // Foreign Keys
    [MaxLength(100)]
    public int Quantity { get; set; }

    public string UserEmail { get; set; }
    public string EventId { get; set; }
    public Event Event { get; set; }
    public User User { get; set; }
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
    [MaxLength(1000)]
    public int VenueCapacity { get; set; }
}

public class Payment
{
    [Key, MaxLength(4)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string PaymentMethod { get; set; }
    [Precision (4, 2)]
    public decimal AmountPaid { get; set; }
    [Column(TypeName = "DATE")]
    public DateOnly PaymentDate { get; set; }

    public string RegistrationId { get; set; }
    public Registration Registration { get; set; }

}
