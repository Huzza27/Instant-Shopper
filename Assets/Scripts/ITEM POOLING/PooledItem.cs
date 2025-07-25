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

        MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
        if (meshFilter == null) return;

        // Assign shared mesh
        Mesh mesh = data.GetSharedMesh();
        meshFilter.sharedMesh = mesh;

        // Compute scale based on bounds
        Vector3 meshSize = mesh.bounds.size;
        Vector3 targetSize = data.BoundsSize;

        if (meshSize.x <= 0 || meshSize.y <= 0 || meshSize.z <= 0) return;

        Vector3 scale = new Vector3(
            targetSize.x / meshSize.x,
            targetSize.y / meshSize.y,
            targetSize.z / meshSize.z
        );

        meshFilter.transform.localScale = scale;

        // Debug logging to validate scaling
        Debug.Log($"PooledItem initialized. Mesh bounds: {meshSize}, Target size: {targetSize}, Scale applied: {scale}");
    }
}
