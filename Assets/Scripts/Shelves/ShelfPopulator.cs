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
        if (itemsToPlace == null || itemsToPlace.Count == 0)
        {
            Debug.LogWarning("No items provided.");
            return;
        }

        BoxCollider shelfCollider = GetComponent<BoxCollider>();
        if (!shelfCollider)
        {
            Debug.LogError("Shelf is missing a BoxCollider.");
            return;
        }

        Bounds shelfBounds = shelfCollider.bounds;
        Vector3 shelfForward = transform.forward;
        Vector3 shelfRight = transform.right;

        Vector3 shelfBackEdge = shelfBounds.center - shelfForward * (shelfBounds.size.z / 2f);

        // Get dimensions of one item
        ShelfItemData selectedItem = itemsToPlace[Random.Range(0, itemsToPlace.Count)].GetItemData();
        GameObject tempInstance = Instantiate(selectedItem.prefab);
        Collider itemCol = tempInstance.GetComponentInChildren<Collider>();

        if (!itemCol)
        {
            Debug.LogError("Item prefab has no collider.");
            Destroy(tempInstance);
            return;
        }

        Bounds itemBounds = itemCol.bounds;
        float itemWidth = itemBounds.size.x;
        float itemDepth = itemBounds.size.z;

        Destroy(tempInstance);

        int itemsPerRow = Mathf.Min(maxItems, Mathf.FloorToInt(shelfBounds.size.x / itemWidth));
        float usableShelfWidth = shelfBounds.size.x;
        float idealSpacing = (itemsPerRow > 1)
            ? (usableShelfWidth - (itemsPerRow * itemWidth)) / (itemsPerRow - 1)
            : 0f;
        float startX = -usableShelfWidth / 2f + itemWidth / 2f;

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

                Bounds instBounds = col.bounds;

                // Align back face with shelf back
                Vector3 localBack = instBounds.center - instBounds.extents.z * shelfForward;
                float pivotToBack = Vector3.Dot(localBack - instance.transform.position, shelfForward);
                Vector3 backAlignmentOffset = shelfForward * (-pivotToBack);

                // Final position (X and Z only)
                Vector3 rightOffset = shelfRight * (startX + i * (itemWidth + idealSpacing));
                Vector3 forwardRowOffset = shelfForward * (layer * (itemDepth + depthLayerSpacing));
                Vector3 spawnPos = shelfBackEdge + rightOffset + forwardRowOffset + backAlignmentOffset;

                instance.transform.position = spawnPos;
                instance.transform.localPosition = new Vector3(
                    instance.transform.localPosition.x,
                    0f,
                    instance.transform.localPosition.z
                );
            }
            shelfSlot.SetMaxItemsForShelfOnInitialPopulation(itemsPerRow * numberOfDepthLayers);
        }
    }
}
