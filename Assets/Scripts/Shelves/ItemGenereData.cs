using System.Collections.Generic;
using UnityEngine;

public class ItemGenereData : MonoBehaviour
{
    [SerializeField] List<ShelfItem> items;

    public List<ShelfItem> GetItems()
    {
        return items;
    }
}
