using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DEB.Models;

#nullable disable warnings

public class DB(DbContextOptions options) : DbContext(options)
{
    // DbSet
    public DbSet<Program> Programs { get; set; }
    public DbSet<Student> Students { get; set; }
}

// Entity Classes -------------------------------------------------------------

public class Program
{
    // Column
    [Key, MaxLength(3)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    // Navigation
    public List<Student> Students { get; set; } = [];
}

public class Student
{
    // Column
    [Key, MaxLength(10)]
    public string Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(1)]
    public string Gender { get; set; }

    // FK
    public string ProgramId { get; set; }

    // Navigation
    public Program Program { get; set; }
}
