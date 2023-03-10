using System.ComponentModel;

namespace Homework.Models.Category
{
    public class CategoryEditingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        [DisplayName("Unit Name")]
        public string UnitName { get; set; } = null!;
        [DisplayName("Parent Category")]
        public int? ParentCategoryId { get; set; }
        public virtual IList<Data.Entities.Category>? ChildCategories { get; set; }
        public virtual IList<Data.Entities.Product>? Products { get; set; }
    }
}