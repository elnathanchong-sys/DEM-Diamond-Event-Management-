using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable warnings

namespace DEM__Diamond_Event_Management_.Models
{
    public class EventVM
    {
        [StringLength(6)]
        public string? Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
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
        [StringLength(100)]
        public string? PosterImg { get; set; }
        [StringLength(100)]
        public string? BannerImg { get; set; }
        [StringLength(10)]
        public string Status { get; set; }
        [Range(0, 1000)]
        public int TotalTickets { get; set; }
        [Range(0, 1000)]
        public int TicketSold { get; set; }
        [Range(1, 10)]
        public int MaxPlayersPerTeam { get; set; }
        [Range(1, 1000)]
        public int MaxTeams { get; set; }
        [Precision(10, 2)]
        public decimal PrizePool { get; set; }



        public string VenueId { get; set; }
        public string CategoryId { get; set; }
        public int TotalEvents { get; set; }
        public int TotalOngoing { get; set; }
        public int TotalPending { get; set; }

        public decimal TotalRevenue { get; set; }
        public int RegisteredPeople { get; set; }

        public List<EventVM> RecentEvents { get; set; } = [];
        public List<EventVM> TrendingEvents { get; set; } = [];
    }
}
