using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DEM.Models;

#nullable disable warnings

public class DB(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users{ get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<Staff> Staff { get; set; }
    public DbSet<Category> Categories { get; set; }
}

public class User
{
    // Column
    [Key, MaxLength(4)]
    public string UserID { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string Email { get; set; }
    [MaxLength(100)]
    public string Password { get; set; }
    [MaxLength(100)]
    public string Phone { get; set; }
    [MaxLength(1)]
    public string Gender { get; set; }
    public string Role => GetType().Name;

    // Navigation
    public List<Registration> Registrations { get; set; } = [];
}

public class Customer : User
{

}

public class Staff : User
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
    [MaxLength(4)]
    public string CustID { get; set; }

    [MaxLength(4)]
    public string EventID { get; set; }

    [ForeignKey("CustID")]
    public Customer Customer { get; set; }
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