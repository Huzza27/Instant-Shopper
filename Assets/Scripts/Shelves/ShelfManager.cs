using System.Collections.Generic;
using UnityEngine;

public class ShelfManager : MonoBehaviour
{
    public static ShelfManager Instance { get; private set; }
    [SerializeField] List<ShelfItem> allItems;
    [SerializeField] List<Shelf> shelves;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<ShelfItem> GetAllItems()
    {
        return allItems;
    }

    public List<Shelf> GetShelves()
    {
        return shelves;
    }
    
    public Shelf GetRandomShelf()
    {
        return shelves[Random.Range(0, shelves.Count)];
    }
}
