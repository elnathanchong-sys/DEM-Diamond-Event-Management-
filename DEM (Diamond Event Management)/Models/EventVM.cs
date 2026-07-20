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
        [StringLength(10)]
        public string EventName { get; set; }
        [Precision(4, 2)]
        public string Price { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
        public DateOnly StartDate { get; set; }
        [Column(TypeName = "DATE")]
        public DateOnly EndDate { get; set; }
        [Precision(4, 2)]
        [Column(TypeName = "TIME")]
        public TimeOnly StartTime { get; set; }
        [Column(TypeName = "TIME")]
        public TimeOnly EndTime { get; set; }
        [MaxLength(100)]
        public string PosterImg { get; set; }
        [MaxLength(100)]
        public string BannerImg { get; set; }

        public string VenueId { get; set; }
        public string CategoryId { get; set; }
    }
}
