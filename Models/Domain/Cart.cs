namespace Homework.Models.Domain;

public class Cart
{
    public IList<CartItem> Items { get; set; } = new List<CartItem>();

    public void AddItem(Data.Entities.Product product)
    {
        var item = GetItemOrDefault(product);
        if (item is not null)
        {
            AddItemCount(item, 1);
        }
        else
        {
            Items.Add(new CartItem { Product = product });
        }
    }

    /// <exception cref="ArgumentNullException">If cart doesn't contain passed <paramref name="product"/></exception>
    public void RemoveItem(Data.Entities.Product product)
    {
        var foundItem = GetItemOrDefault(product);
        if (foundItem is null)
            throw new ArgumentNullException(nameof(product));

        Items.Remove(foundItem);
    }

    public void Clear()
    {
        Items.Clear();
    }
    
    public void RemoveItem(CartItem cartItem)
    {
        RemoveItem(cartItem.Product);
    }
    
    /// <exception cref="ArgumentNullException">If cart doesn't contain passed <paramref name="cartItem"/></exception>
    public void AddItemCount(CartItem cartItem, int countToAdd)
    {
        if (cartItem is null)
            throw new ArgumentNullException(nameof(cartItem));

        var newCount = cartItem.Count + countToAdd;
        if (newCount <= cartItem.Product.Count)
            cartItem.Count = newCount;
    }

    /// <exception cref="ArgumentNullException">If cart doesn't contain passed <paramref name="cartItem"/></exception>
    public void SubtractItemCount(CartItem cartItem, int countToSubtract)
    {
        if (cartItem is null)
            throw new ArgumentNullException(nameof(cartItem));

        var newCount = cartItem.Count - countToSubtract;
        cartItem.Count = Math.Max(0, newCount);
    }
    
    public CartItem? GetItemOrDefault(Data.Entities.Product product)
    {
        var cartItem = Items.FirstOrDefault(item => item.Product.Id == product.Id);
        return cartItem;
    }

    private CartItem? GetItemOrDefault(CartItem item)
    {
        var foundItem = GetItemOrDefault(item.Product);
        return foundItem;
    }

}