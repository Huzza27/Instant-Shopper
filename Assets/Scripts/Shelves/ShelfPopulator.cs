using System.Collections.Generic;
using UnityEngine;

public class ShelfPopulator : MonoBehaviour
{
    public float itemSpacing = 0.02f;
    public float depthLayerSpacing = 0.05f;
    public int maxItems = 10;
    public int numberOfDepthLayers = 3;
    [SerializeField] private ShelfSlot shelfSlot;

    void Start()
    {
        shelfSlot = GetComponent<ShelfSlot>();
    }

    public void FillShelfFromList(List<ShelfItem> itemsToPlace)
    {
        if (!IsValidItemList(itemsToPlace)) return;

        if (!TryGetComponent(out BoxCollider shelfCollider))
        {
            Debug.LogError("Shelf is missing a BoxCollider.");
            return;
        }

        Bounds shelfBounds = shelfCollider.bounds;
        Vector3 shelfForward = transform.forward;
        Vector3 shelfRight = transform.right;
        Vector3 shelfBackEdge = shelfBounds.center - shelfForward * (shelfBounds.size.z / 2f);

        ShelfItemData selectedItem = ChooseRandomItem(itemsToPlace);
        Vector2 itemSize = GetItemDimensions(selectedItem);

        int itemsPerRow = CalculateItemsPerRow(shelfBounds.size.x, itemSize.x);
        float idealSpacing = CalculateIdealSpacing(shelfBounds.size.x, itemSize.x, itemsPerRow);
        float startX = CalculateStartX(shelfBounds.size.x, itemSize.x);

        PopulateShelf(selectedItem, shelfBounds, shelfForward, shelfRight, shelfBackEdge, itemSize, itemsPerRow, idealSpacing, startX);
        shelfSlot.SetMaxItemsForShelfOnInitialPopulation(itemsPerRow * numberOfDepthLayers);
    }

    private bool IsValidItemList(List<ShelfItem> itemsToPlace)
    {
        if (itemsToPlace == null || itemsToPlace.Count == 0)
        {
            Debug.LogWarning("No items provided.");
            return false;
        }
        return true;
    }

    private ShelfItemData ChooseRandomItem(List<ShelfItem> items)
    {
        return items[Random.Range(0, items.Count)].GetItemData();
    }

    private Vector2 GetItemDimensions(ShelfItemData itemData)
    {
        GameObject tempInstance = Instantiate(itemData.prefab);
        Collider itemCol = tempInstance.GetComponentInChildren<Collider>();

        if (!itemCol)
        {
            Debug.LogError("Item prefab has no collider.");
            Destroy(tempInstance);
            return Vector2.zero;
        }

        Bounds itemBounds = itemCol.bounds;
        Vector2 size = new Vector2(itemBounds.size.x, itemBounds.size.z);
        Destroy(tempInstance);
        return size;
    }

    private int CalculateItemsPerRow(float shelfWidth, float itemWidth)
    {
        return Mathf.Min(maxItems, Mathf.FloorToInt(shelfWidth / itemWidth));
    }

    private float CalculateIdealSpacing(float shelfWidth, float itemWidth, int itemsPerRow)
    {
        return (itemsPerRow > 1) ? (shelfWidth - (itemsPerRow * itemWidth)) / (itemsPerRow - 1) : 0f;
    }

    private float CalculateStartX(float shelfWidth, float itemWidth)
    {
        return -shelfWidth / 2f + itemWidth / 2f;
    }

    private void PopulateShelf(ShelfItemData selectedItem, Bounds shelfBounds, Vector3 shelfForward, Vector3 shelfRight, Vector3 shelfBackEdge, Vector2 itemSize, int itemsPerRow, float idealSpacing, float startX)
    {
        for (int layer = 0; layer < numberOfDepthLayers; layer++)
        {
            for (int i = 0; i < itemsPerRow; i++)
            {
                GameObject instance = Instantiate(selectedItem.prefab, transform);
                Collider col = instance.GetComponentInChildren<Collider>();
                shelfSlot.AddItem(instance.GetComponent<ShelfItem>());

                if (!col)
                {
                    Destroy(instance);
                    continue;
                }

                Vector3 spawnPos = CalculateItemPosition(instance, col, shelfForward, shelfRight, shelfBackEdge, startX, i, itemSize, idealSpacing, layer);

                instance.transform.position = spawnPos;
                instance.transform.localPosition = new Vector3(
                    instance.transform.localPosition.x,
                    0f,
                    instance.transform.localPosition.z
                );



                ShelfItemVisualUtility.ApplySharedVisuals(instance);
                instance.GetComponent<Collider>().enabled = false;
            }
        }
    }

    private Vector3 CalculateItemPosition(GameObject instance, Collider col, Vector3 shelfForward, Vector3 shelfRight, Vector3 shelfBackEdge, float startX, int index, Vector2 itemSize, float spacing, int layer)
    {
        Bounds instBounds = col.bounds;

        Vector3 localBack = instBounds.center - instBounds.extents.z * shelfForward;
        float pivotToBack = Vector3.Dot(localBack - instance.transform.position, shelfForward);
        Vector3 backAlignmentOffset = shelfForward * (-pivotToBack);

        Vector3 rightOffset = shelfRight * (startX + index * (itemSize.x + spacing));
        Vector3 forwardRowOffset = shelfForward * (layer * (itemSize.y + depthLayerSpacing));

        return shelfBackEdge + rightOffset + forwardRowOffset + backAlignmentOffset;
    }
}
