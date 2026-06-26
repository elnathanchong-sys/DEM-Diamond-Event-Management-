using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Models;

public class DB(DbContextOptions options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Registration> Registrations { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<Society> Societies { get; set; }
}

public class Customer
{
    // Column
    [Key, MaxLength(4)]
    public string CustId { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(100)]
    public string email { get; set; }
    [MaxLength(100)]
    public string password { get; set; }
    [MaxLength(100)]
    public string phone { get; set; }
    [MaxLength(1)]
    public string gender { get; set; }

    // Navigation
    public List<Registration> Registration { get; set; } = [];
}

public class Event
{
    // Column
    [Key, MaxLength(4)]
    public string EventID { get; set; }
    [MaxLength(100)]
    public string EventName { get; set; }
    [MaxLength(100)]
    public string EventDate { get; set; }
    [MaxLength(100)]
    public string Description { get; set; }
    [MaxLength(100)]
    public string venue { get; set; }

    // Foreign Key
    [MaxLength(4)]
    public string SocietyID { get; set; }

    [ForeignKey("SocietyID")]
    public Society Society { get; set; }

    // Navigation
    public List<Registration> Registration { get; set; } = [];
}

public class Registration
{
    // Column
    [Key, MaxLength(4)]
    public string regID { get; set; }
    [MaxLength(100)]
    public string regDate { get; set; }
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

public class Staff
{
    // Column
    [Key, MaxLength(4)]
    public string StaffID { get; set; }
    [MaxLength(100)]
    public string StaffName { get; set; }
    [MaxLength(100)]
    public string email { get; set; }
    [MaxLength(100)]
    public string password { get; set; }
    [MaxLength(50)]
    public string role { get; set; }

    // Navigation
    public List<Society> Societies { get; set; } = [];
}

public class Society
{
    // Column
    [Key, MaxLength(4)]
    public string SocietyID { get; set; }
    [MaxLength(100)]
    public string societyName { get; set; }
    [MaxLength(100)]
    public string description { get; set; }
    [MaxLength(100)]
    public string category { get; set; }
    [MaxLength(100)]
    public string StaffID { get; set; }

    //ForeignKey
    [MaxLength(4)]
    public Staff Staff { get; set; }


    // Navigation
    public List<Event> Events { get; set; } = [];
}