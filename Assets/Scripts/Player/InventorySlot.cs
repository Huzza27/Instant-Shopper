using TMPro;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] ShelfItem itemInSlot;

    [SerializeField] private TextMeshProUGUI tempItemName;
    [SerializeField] int slotIndex;

    public bool GetIsSlotInUse()
    {
        return itemInSlot != null;
    }

    public ShelfItem GetItemInSlot()
    {
        return itemInSlot;
    }

    public void SetItem(ShelfItem newItem)
    {
        itemInSlot = newItem;
    }

    public void UpdateSlotUIOnItemPickup()
    {
        tempItemName.text = itemInSlot.GetItemData().itemName;
        tempItemName.color = Color.green;
    }

    public void UpdateSlotUIOnCycle(InventorySlot slot)
    {
        if (slot == this)
            tempItemName.color = Color.green;
        else
            tempItemName.color = Color.black;
    }

    public int GetSlotIndex()
    {
        return slotIndex;
    }

    public void ClearSlot()
    {
        tempItemName.color = Color.black;
        tempItemName.text = "None";
    }
}
