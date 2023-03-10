using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Homework.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        [DisplayName("Manufacturer")]
        public int ManufacturerId { get; set; }
        public virtual Manufacturer Manufacturer { get; set; } = null!;
        [Range(0, 5)]
        public int Rating { get; set; }
        [Range(1, 1000000)]
        public double Price { get; set; }
        [Range(0, 1000000)]
        public int Count { get; set; }
        [DisplayName("Category")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; } = null!;

        public virtual IList<ProductImage> Images { get; set; } = null!;
        public virtual IList<BannerImage>? AssociatedBanners { get; set; } = null!;
    }
}