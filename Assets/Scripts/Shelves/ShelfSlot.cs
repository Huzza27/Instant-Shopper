using System.Collections.Generic;
using UnityEngine;
public enum ShelfSize { Small, Medium, Large }

public class ShelfSlot : MonoBehaviour
{
    public ShelfSize allowedSize;
    [SerializeField] private int maxItems;
    [SerializeField] List<ShelfItem> itemsInSlot;
    [SerializeField] private Transform aiShelfTransform;
    [SerializeField] private ShelfPopulator populator;


    public void FillShelfFromList(List<ShelfItem> items)
    {
        populator.FillShelfFromList(items);
    }

    public void AddItem(ShelfItem item)
    {
        itemsInSlot.Add(item);
    }

    public void SetMaxItemsForShelfOnInitialPopulation(int maxItems)
    {
        this.maxItems = maxItems;
    }
    public Transform GetAIShelfTransform()
    {
        return aiShelfTransform;
    }

    public void RemoveItemFromSlot()
    {
        ShelfItem item = itemsInSlot[0];
        itemsInSlot.Remove(item);
        Destroy(item.gameObject); //will add placing item in cart
    }
    
    public List<ShelfItem> GetItemsInSlot()
    {
        return itemsInSlot;
    }
}
