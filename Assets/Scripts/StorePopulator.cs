using System.Collections.Generic;
using UnityEngine;

public class StorePopulator : MonoBehaviour
{
    [SerializeField] List<ShelfItem> allItems;
    [SerializeField] List<Shelf> shelves;

    void Start()
    {
        foreach (Shelf shelf in shelves)
        {
            shelf.PopulateShelf(allItems);
        }
    }
}
