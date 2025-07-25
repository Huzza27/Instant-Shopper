using System.Collections.Generic;
using UnityEngine;

public class ItemPoollingManager : MonoBehaviour
{
    [SerializeField] List<ShelfItem> shelfItems;

    void Awake()
    {
        GenerateItemPool();
    }

    public void GenerateItemPool()
    {
        foreach (ShelfItem item in shelfItems)
        {
            GameObject newItem = Instantiate(item.gameObject, transform.position, Quaternion.identity, transform);
            newItem.GetComponent<PooledItem>().Initialize(item.GetItemData());
        }
    }
}
