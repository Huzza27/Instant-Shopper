using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class Shopper : MonoBehaviour
{
    [SerializeField] public UnityEvent<ICart> OnShoppingCartInteract;
    [SerializeField] public FollowCamera cameraFollowScript;
    [SerializeField] public FPSCamera fpsCamera;
    [SerializeField] private bool isHoldingSomething;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform itemHoldingPoint;
    [SerializeField] ICart cart;

    void Start()
    {
        inventory.OnNewItemEquip += DisplayItemInHand;
    }

    public bool GetIsPlayerHoldingSomething()
    {
        return inventory.GetCurrentSlot().GetIsSlotInUse();
    }

    public ShelfItem GetHeldItem()
    {
        return inventory.GetHeldItem();
    }

    public void AddItemToInventory(ShelfItem item)
    {
        inventory.AddItemToInventory(item);
    }

    public void DisplayItemInHand(ShelfItem newItem)
    {
        if(inventory.GetPlayerHasItem(newItem) || newItem == null)
        {
            SwapItems(newItem, inventory.GetCurrentItem());
            return;
        }
        MoveItemToHand(newItem);
    }

    public ShelfItem CreateItemClone(ShelfItem item)
    {
        GameObject newItem = Instantiate(item.gameObject, itemHoldingPoint.position, Quaternion.identity);
        newItem.transform.SetParent(itemHoldingPoint);
        return newItem.GetComponent<ShelfItem>();
    }

    public void SwapItems(ShelfItem newItem, ShelfItem currentItem)
    {

        if (currentItem != null)
        {
            currentItem.gameObject.SetActive(false);
        }
        if (newItem != null)
        {
            newItem.gameObject.SetActive(true);
        }
    }

    public void MoveItemToHand(ShelfItem item)
    {
        item.transform.position = itemHoldingPoint.position;
        item.transform.SetParent(itemHoldingPoint.transform);
    }
    public void TryPickupItem(ShelfItem item)
    {
        if (!inventory.CanPickUpItem()) return;
        ShelfItem newItem = item.CreateItemClone();
        MoveItemToHand(newItem);
        AddItemToInventory(newItem);
    }

    public void RemoveItemFromInventory(ShelfItem item)
    {
        inventory.RemoveItemFromInventory(item);
    }

    public void SetCart(ICart newCart)
    {
        cart = newCart;
    }

    public ICart GetCart()
    {
        return cart;
    }
}
