using System.ComponentModel.DataAnnotations;

namespace DEM__Diamond_Event_Management_.Models
{
    public class IndexBannerVM
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        [StringLength(100)]
        public string? IndexBannerImg { get; set; }
    }
}