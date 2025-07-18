using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Inventory : MonoBehaviour
{
    [SerializeField] List<InventorySlot> slots;
    [SerializeField] private List<ShelfItem> items = new List<ShelfItem>();
    [SerializeField] int maxSlots;
    [SerializeField] int numSlotsTaken;
    [SerializeField] InventorySlot currentSlot;
    [SerializeField] ShelfItem currentlyEquippedItem;
    [SerializeField] InputAction mouseWheel;
    public Action<ShelfItem> OnNewItemEquip; //This event calls DisplayItemInHand in Shopper.cs

    void Start()
    {
        mouseWheel.Enable();
        mouseWheel.performed += OnScroll;
    }

    public InventorySlot GetCurrentSlot()
    {
        return currentSlot;
    }

    public ShelfItem GetShelfItemInSlot(int slotIndex)
    {
        return slots[slotIndex].GetItemInSlot();
    }

    public ShelfItem GetHeldItem()
    {
        return currentSlot.GetItemInSlot();
    }

    public bool CanPickUpItem()
    {
        return numSlotsTaken < maxSlots;
    }

    public void AddItemToInventory(ShelfItem item)
    {
        items.Add(item);
        InventorySlot openSlot = GetNextOpenSlot();
        openSlot.SetItem(item);
        openSlot.UpdateSlotUIOnItemPickup();
        currentSlot = openSlot; //Gonna swap to that slot anyway, why not just set it now
    }

    public void OnScroll(InputAction.CallbackContext ctx)
    {
        Vector2 scrollDelta = ctx.ReadValue<Vector2>();
        CycleInventory(scrollDelta.y);

    }

    public void CycleInventory(float scrollDelta)
    {
        currentlyEquippedItem = currentSlot.GetItemInSlot();
        currentSlot = GetNewSlotOnMouseCylce(scrollDelta);
        EquipItem(currentSlot.GetItemInSlot());
        UpdateSlotUI();
    }

    public void UpdateSlotUI()
    {
        foreach (InventorySlot slot in slots)
        {
            slot.UpdateSlotUIOnCycle(currentSlot);
        }
    }

    public InventorySlot GetNewSlotOnMouseCylce(float scrollDelta)
    {
        int direction = scrollDelta > 0 ? 1 : -1;

        int newIndex = currentSlot.GetSlotIndex() + direction;

        // Wrap around if needed
        if (newIndex < 0)
            newIndex = slots.Count - 1;
        else if (newIndex >= slots.Count)
            newIndex = 0;
        Debug.Log("Slot index = " + newIndex);
        return slots[newIndex];
    }

    public void EquipItem(ShelfItem newItem)
    {
        OnNewItemEquip?.Invoke(newItem);
    }


    public InventorySlot GetNextOpenSlot()
    {
        foreach (InventorySlot slot in slots)
        {
            if (!slot.GetIsSlotInUse())
            {
                return slot;
            }
        }
        return null; //Add visual cue for inventory full
    }

    public bool GetPlayerHasItem(ShelfItem item)
    {
        return items.Contains(item);
    }

    public ShelfItem GetCurrentItem()
    {
        return currentlyEquippedItem;
    }

    public void RemoveItemFromInventory(ShelfItem item)
    {
        InventorySlot slot = GetSlotThatHasItem(item);
        slot.ClearSlot();

    }

    public InventorySlot GetSlotThatHasItem(ShelfItem item)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.GetItemInSlot() == item)
            {
                return slot;
            }
        }
        return null;
    }
}
