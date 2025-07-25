using UnityEngine;

public class ItemSpawnerService : MonoBehaviour, IItemSpawner
{
    public static IItemSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public GameObject SpawnItem(ShelfItemData data, Transform parent)
    {
        return Instantiate(data.prefab, parent.position, Quaternion.identity, parent);
    }
}
