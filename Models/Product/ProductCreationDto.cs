using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Homework.Models.Product
{
    public class ProductCreationDto
    {
        public string Title { get; set; } = null!;
        [DisplayName("Manufacturer")]
        public int ManufacturerId { get; set; }
        [Range(0, 5)]
        public int Rating { get; set; }
        [Range(1, 1000000)]
        public double Price { get; set; }
        [Range(0, 1000000)]
        public int Count { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
    }
}