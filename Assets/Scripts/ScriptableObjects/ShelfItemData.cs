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

public enum ItemShape
{
    Box,
    Bottle,
    Can,
    Jug,
    Bag
}
[System.Serializable]
public class ShelfItemData : MonoBehaviour
{
    [HideInInspector] public string itemName;
    public GameObject prefab;
    [SerializeField] private SharedMeshLibrary sharedMeshLibrary;
    public Sprite icon;
    public float weight;
    public ItemType itemType;
    public ItemShape itemShape;
    public ShelfSize size;
    public Vector3 visualOffset;
    public Vector3 visualScale = Vector3.one;

    public Vector2 UVOffset;
    public Vector2 UVScale;
    public Vector3 BoundsSize;
    public Material sharedMaterial;

    void Awake()
    {
        itemName = gameObject.name;
        prefab = gameObject;
        sharedMaterial = GetComponent<MeshRenderer>()?.material;

        MeshFilter mf = GetComponentInChildren<MeshFilter>();
        if (mf != null)
        {
            Mesh mesh = mf.sharedMesh;
            ExtractUVInfoFromMesh(mesh);
        }
    }

    void OnEnable()
    {
        BoundsSize = GetComponent<Collider>().bounds.size;
    }

    public Mesh GetSharedMesh()
    {
        return sharedMeshLibrary.GetMesh(itemShape);
    }

    public Material GetSharedMaterial()
    {
        return sharedMaterial;
    }

    public void ExtractUVInfoFromMesh(Mesh mesh)
    {
        Vector2[] uvs = mesh.uv;
        if (uvs == null || uvs.Length == 0) { UVOffset = Vector2.zero; UVScale = Vector2.one; return; }

        Vector2 minUV = uvs[0], maxUV = uvs[0];
        foreach (Vector2 uv in uvs)
        {
            minUV = Vector2.Min(minUV, uv);
            maxUV = Vector2.Max(maxUV, uv);
        }

        UVOffset = minUV;
        UVScale = maxUV - minUV;
    }
}
