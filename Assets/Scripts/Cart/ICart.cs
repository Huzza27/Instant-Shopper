using System.Collections.Generic;
using UnityEngine;

public interface ICart
{
    Transform GetDropTransform();
    Transform GetCartTransform();
    List<ShelfItem> GetItems();
    void PlaceItemInShoppingCart(ShelfItem item);
    void OnItemPlaced(GameObject item);
    public void ClearCartInventory();
}
