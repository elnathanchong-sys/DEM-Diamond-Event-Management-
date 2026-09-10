using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable warnings

namespace DEM__Diamond_Event_Management_.Models
{
    public class CategoryVM
    {
        [StringLength(4)]
        public string Id { get; set; } = "";

        [StringLength(100)]
        public string Name { get; set; }

    }
}