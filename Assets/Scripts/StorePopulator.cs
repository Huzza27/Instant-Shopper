using System.Collections.Generic;
using UnityEngine;

public class StorePopulator : MonoBehaviour
{
    public StorePopulator instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (Shelf shelf in ShelfManager.Instance.GetShelves())
        {
            shelf.PopulateShelf(ShelfManager.Instance.GetAllItems());
        }
    }  
}
