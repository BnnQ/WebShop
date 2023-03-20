using Homework.Data.Entities;
using X.PagedList;

namespace Homework.ViewModels.Shop;

public class ListViewModel
{
    public Category? Category { get; set; }
    public IPagedList<Product> Products { get; set; }
    public int NumberOfProducts { get; set; }

    public ListViewModel(Category? category, IPagedList<Product> products, int numberOfProducts)
    {
        Category = category;
        Products = products;
        NumberOfProducts = numberOfProducts;
    }
}