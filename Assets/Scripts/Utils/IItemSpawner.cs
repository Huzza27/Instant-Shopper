using UnityEngine;

public interface IItemSpawner
{
    GameObject SpawnItem(ShelfItemData data, Transform parent);
}
