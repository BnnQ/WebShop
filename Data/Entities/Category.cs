using System.ComponentModel;

namespace Homework.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string UnitName { get; set; } = null!;
        [DisplayName("Parent Category")]
        public int? ParentCategoryId { get; set; }
        [DisplayName("Parent Category")]
        public virtual Category? ParentCategory { get; set; }
        public virtual IList<Category>? ChildCategories { get; set; }
        public virtual IList<Product>? Products { get; set; }
    }
}