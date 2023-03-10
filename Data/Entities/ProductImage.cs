namespace Homework.Data.Entities
{
    public class ProductImage : Image
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}