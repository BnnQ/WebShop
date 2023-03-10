namespace Homework.Data.Entities
{
    public class Manufacturer
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public virtual IList<Product>? Products { get; set; }
    }
}