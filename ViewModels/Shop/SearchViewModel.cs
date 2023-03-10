using Homework.Data.Entities;

namespace Homework.ViewModels.Shop
{
    public class SearchViewModel
    {
        public IEnumerable<Product> Products { get; set; } = null!;
        public string Query { get; set; } = null!;
    }
}