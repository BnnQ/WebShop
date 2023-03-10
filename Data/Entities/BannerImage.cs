namespace Homework.Data.Entities
{
    public class BannerImage : Image
    {
        public int? AssociatedProductId { get; set; }
        public Product? AssociatedProduct { get; set; }
    }
}