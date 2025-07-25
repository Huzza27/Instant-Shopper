using UnityEngine;

[CreateAssetMenu(fileName = "SharedMeshLibrary", menuName = "Mesh/SharedMeshLibrary")]
public class SharedMeshLibrary : ScriptableObject
{
    public Mesh boxMesh;
    public Mesh bottleMesh;
    public Mesh canMesh;
    public Mesh jugMesh;
    public Mesh bagMesh;

    public Mesh GetMesh(ItemShape shape)
    {
        return shape switch
        {
            ItemShape.Box => boxMesh,
            ItemShape.Bottle => bottleMesh,
            ItemShape.Can => canMesh,
            ItemShape.Jug => jugMesh,
            ItemShape.Bag => bagMesh,
            _ => null
        };
    }
}
