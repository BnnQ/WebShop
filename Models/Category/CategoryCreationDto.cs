using System.ComponentModel;

namespace Homework.Models.Category
{
    public class CategoryCreationDto
    {
        public string Name { get; set; } = null!;
        [DisplayName("Unit Name")]
        public string UnitName { get; set; } = null!;
        [DisplayName("Parent Category")]
        public int? ParentCategoryId { get; set; }
    }
}