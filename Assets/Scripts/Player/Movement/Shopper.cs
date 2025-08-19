using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class Shopper : MonoBehaviour
{
    [SerializeField] public UnityEvent<ICart> OnShoppingCartInteract;
    [SerializeField] public UnityEvent<IDriveable> OnMountDriveable;
    public UnityEvent<Transform> OnSetMovementTarget;
    [SerializeField] public FollowCamera cameraFollowScript;
    [SerializeField] public FPSCamera fpsCamera;
    [SerializeField] private bool isHoldingSomething;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Transform itemHoldingPoint;
    [SerializeField] private Transform handParentObject;
    [SerializeField] private float throwForce;
    [SerializeField] IDriveable driveable;

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
        if (inventory.GetPlayerHasItem(newItem) || newItem == null)
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
        item.transform.SetParent(handParentObject.transform);
    }
    public void TryPickupItem(ShelfItem item)
    {
        if (!inventory.CanPickUpItem()) return;
        ShelfItem newItem = item.CreateItemClone();
        newItem.gameObject.layer = 0;
        Rigidbody rb = newItem.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        MoveItemToHand(newItem);
        AddItemToInventory(newItem);
    }

    public void ThrowObject()
    {
        if (!GetIsPlayerHoldingSomething()) return;
        ShelfItem item = GetHeldItem();
        item.transform.SetParent(null);
        item.ThrowItem(throwForce); // Example throw force
        inventory.RemoveItemFromInventory(item);
    }

    public void MountDriveable(IDriveable driveable)
    {
        LeanTween.move(gameObject, driveable.GetSeatTransform().position, 0.3f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                transform.rotation = Quaternion.LookRotation(driveable.GetSeatTransform().forward);
            });
    }
    public void RemoveItemFromInventory(ShelfItem item)
    {
        inventory.RemoveItemFromInventory(item);
    }

    public void SetDrivable(IDriveable newDrivable)
    {
        driveable = newDrivable;
    }

    public IDriveable GetDriveable()
    {
        return driveable;
    }

    public FPSCamera GetShopperCamera()
    {
        return fpsCamera;
    }

    public CharacterController GetCharacterController()
    {
        return GetComponent<CharacterController>();
    }
}
