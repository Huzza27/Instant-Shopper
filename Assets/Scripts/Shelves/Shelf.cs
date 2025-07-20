using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    private List<ShelfSlot> shelfSlots;

    void Awake()
    {
        shelfSlots = new List<ShelfSlot>();

        foreach (ShelfSlot slot in GetComponentsInChildren<ShelfSlot>())
        {
            shelfSlots.Add(slot);
        }
    }

    public void PopulateShelf(List<ShelfItem> itemsToPlace)
    {
        foreach (ShelfSlot slot in shelfSlots)
        {
            slot.FillShelfFromList(itemsToPlace);
        }
    }

    public ShelfSlot GetRandomSlot()
    {
        return shelfSlots[Random.Range(0, shelfSlots.Count)];
    }
}
