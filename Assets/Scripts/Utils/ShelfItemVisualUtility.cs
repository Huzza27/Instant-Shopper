using UnityEngine;

public static class ShelfItemVisualUtility
{
    public static void ApplySharedVisuals(GameObject instance)
    {
        ShelfItem shelfItem = instance.GetComponent<ShelfItem>();
        if (shelfItem == null) return;

        Mesh sharedMesh = shelfItem.GetItemData().GetSharedMesh();

        // Store collider size BEFORE swapping mesh
        Collider col = instance.GetComponentInChildren<Collider>();
        if (col == null) return;

        Vector3 originalSize = col.bounds.size;

        ReplaceMesh(instance, sharedMesh);
        RescaleMesh(instance, sharedMesh, originalSize);
    }

    private static void ReplaceMesh(GameObject item, Mesh mesh)
    {
        MeshFilter mf = item.GetComponentInChildren<MeshFilter>();
        if (mf == null) return;

        mf.sharedMesh = mesh;
    }

    private static void RescaleMesh(GameObject item, Mesh mesh, Vector3 targetSize)
    {
        MeshFilter mf = item.GetComponentInChildren<MeshFilter>();
        if (mf == null || mesh == null) return;

        Vector3 meshSize = mesh.bounds.size;

        Vector3 scale = new Vector3(
            targetSize.x / meshSize.x,
            targetSize.y / meshSize.y,
            targetSize.z / meshSize.z
        );

        mf.transform.localScale = scale;
    }
}
