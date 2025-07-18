using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    private List<GameObject> shelveSlots;

    void Awake()
    {
        shelveSlots = new List<GameObject>();

        foreach (ShelfPopulator populator in GetComponentsInChildren<ShelfPopulator>())
        {
            shelveSlots.Add(populator.gameObject);
        }
    }

    public void PopulateShelf(List<ShelfItem> itemsToPlace)
    {
        foreach (GameObject slot in shelveSlots)
        {
            slot.GetComponent<ShelfPopulator>().FillShelfFromList(itemsToPlace);
        }

    }
}
