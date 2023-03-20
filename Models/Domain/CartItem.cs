using Homework.Models.Product;

namespace Homework.Models.Domain;

public class CartItem
{
    public Data.Entities.Product Product { get; set; } = null!;
    public int Count { get; set; } = 1;
}