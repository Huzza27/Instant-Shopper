using UnityEngine;

public class PooledItem : MonoBehaviour
{
    void Start()
    {
        Initialize(GetComponent<ShelfItemData>());
    }

    public void Initialize(ShelfItemData data)
    {
        if (data == null) return;

        // Get MeshFilter
        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter == null) return;

        // Get the shared base mesh (normalized UVs)
        Mesh sharedMesh = data.GetSharedMesh();
        if (sharedMesh == null) return;

        // Make a copy so we can modify UVs safely
        Mesh meshInstance = Instantiate(sharedMesh);
        meshFilter.sharedMesh = meshInstance;

        // Scale mesh to match desired bounding box
        Vector3 meshSize = meshInstance.bounds.size;
        Vector3 targetSize = data.GetBoundsSize;

        if (meshSize.x <= 0 || meshSize.y <= 0 || meshSize.z <= 0) return;

        Vector3 scale = new Vector3(
            targetSize.x / meshSize.x,
            targetSize.y / meshSize.y,
            targetSize.z / meshSize.z
        );

        meshFilter.transform.localScale = scale;

        // 🟩 Apply per-item UV region from ShelfItemData (UVOffset + UVScale)
        ApplyUVAdjustment(meshInstance, data.UVOffset, data.UVScale);

        // 🟩 Use shared material for batching
        Renderer renderer = meshFilter.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = data.GetSharedMaterial();
        }

        // 🟨 Rescale collider to match new mesh bounds
        Collider col = GetComponent<Collider>();
        if (col is BoxCollider boxCollider)
        {
            boxCollider.center = meshInstance.bounds.center;
            boxCollider.size = meshInstance.bounds.size;
        }
        else if (col is MeshCollider meshCollider)
        {
            meshCollider.sharedMesh = meshInstance; // Re-assign to update bounds
        }

        Debug.Log($"[PooledItem] {data.name} initialized. Offset: {data.UVOffset}, Scale: {data.UVScale}, Mesh scale: {scale}");
    }

    private void ApplyUVAdjustment(Mesh mesh, Vector2 offset, Vector2 scale)
    {
        Vector2[] originalUVs = mesh.uv;
        Vector2[] adjustedUVs = new Vector2[originalUVs.Length];

        // Normalize UVs before scaling
        Vector2 minUV = originalUVs[0];
        Vector2 maxUV = originalUVs[0];
        for (int i = 1; i < originalUVs.Length; i++)
        {
            minUV = Vector2.Min(minUV, originalUVs[i]);
            maxUV = Vector2.Max(maxUV, originalUVs[i]);
        }

        Vector2 range = maxUV - minUV;

        for (int i = 0; i < originalUVs.Length; i++)
        {
            Vector2 normalized = originalUVs[i] - minUV;
            if (range.x != 0f) normalized.x /= range.x;
            if (range.y != 0f) normalized.y /= range.y;

            adjustedUVs[i] = Vector2.Scale(normalized, scale) + offset;
        }

        mesh.uv = adjustedUVs;
    }
}
