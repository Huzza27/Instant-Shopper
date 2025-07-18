using UnityEngine;


public enum ItemType
{
    Produce,
    Dairy,
    Bakery,
    Frozen,
    Snacks,
    Household,
    Toiletries
}


[CreateAssetMenu(fileName = "ShelfItem", menuName = "ShelfItem")]
public class ShelfItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public float weight;
    public ItemType itemType;
    public ShelfSize size;
    public Vector3 visualOffset;
    public Vector3 visualScale = Vector3.one;
}
