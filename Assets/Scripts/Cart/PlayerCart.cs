using System.Collections.Generic;
using UnityEngine;
public class PlayerCart : MonoBehaviour, ICart
{
    [SerializeField] private CartItemPlacer itemPlacer;
    [SerializeField] private Transform itemGrid;
    [SerializeField] private Transform playerStandingPoint;
    [SerializeField] private Transform cartCameraMount;
    [SerializeField] Transform rotationPivot;
    [SerializeField] private Rigidbody cartRB;

    public Transform GetDropTransform() => itemGrid;
    public Transform GetCartTransform() => this.transform;

    public Transform GetCameraMount() => cartCameraMount;
    public Transform GetStandingPoint() => playerStandingPoint;
    public Transform GetRotationPivot() => rotationPivot;
    public Rigidbody GetCartRB() => cartRB;
    [SerializeField] private List<ShelfItem> cartIventory = new List<ShelfItem>();

    public void OnItemPlaced(GameObject item)
    {
        item.GetComponent<IPlaceableInCart>().OnPlacedIntoCart(this.transform);
        cartIventory.Add(item.GetComponent<ShelfItem>());
    }
    public void PlaceItemInShoppingCart(ShelfItem item)
    {
        itemPlacer.PlaceInCart(item, this);
    }

    public List<ShelfItem> GetItems()
    {
        return cartIventory;
    }

    public void ClearCartInventory()
    {
        foreach (ShelfItem item in cartIventory)
        {
            Destroy(item.gameObject); // Destroy the item GameObject
        }
        cartIventory.Clear();
    }
}
