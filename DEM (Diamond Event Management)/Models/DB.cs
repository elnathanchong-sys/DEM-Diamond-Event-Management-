using DEM__Diamond_Event_Management_.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEM__Diamond_Event_Management_.Models;

#nullable disable warnings

public class DB(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users{ get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<Admin> Admin { get; set; }
    public DbSet<Category> Category { get; set; }
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
    public string Role => GetType().Name;

    // Navigation
    public List<Registration> Registrations { get; set; } = [];
}

public class Member : User
{

}

public class Admin : User
{

}

public class Event
{
    // Column testing one
    [Key, MaxLength(4)]
    public string EventID { get; set; }
    [MaxLength(100)]
    public string EventName { get; set; }
    [Column(TypeName = "DATE")]
    public DateOnly EventDate { get; set; }
    [MaxLength(100)]
    public string Description { get; set; }
    [MaxLength(100)]
    public string Venue { get; set; }

    //Foreign Key
    [MaxLength(4)]
    public string CategoryID { get; set; }
    [ForeignKey("CategoryID")]
    public Category Category { get; set; }

    // Navigation
    public List<Registration> Registrations { get; set; } = [];
}

public class Registration
{
    // Column
    [Key, MaxLength(4)]
    public string RegID { get; set; }
    [Column(TypeName = "DATE")]
    public DateOnly RegDate { get; set; }
    [MaxLength(20)]
    public string Status { get; set; }
    // Foreign Keys
    [MaxLength(100)]
    public string Email { get; set; }

    [MaxLength(4)]
    public string EventID { get; set; }

    [ForeignKey("Email")]
    public Member Members { get; set; }
    [ForeignKey("EventID")]
    public Event Event { get; set; }
}

public class Category
{
    // Column
    [Key, MaxLength(4)]
    public string CategoryID { get; set; }
    [MaxLength(100)]
    public string CategoryName { get; set; }

    // Navigation
    public List<Event> Events { get; set; } = [];
}