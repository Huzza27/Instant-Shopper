using UnityEngine;

public class ShelfItem : MonoBehaviour, IInteractable
{
    private Rigidbody rb;
    [SerializeField] ShelfItemData data;
    [SerializeField] Vector3 cloneScale;
    public void Interact(InteractionContexzt context, Shopper currentShopper)
    {
        if (PlayerStateManager.Instance.GetPlayerState() == PlayerState.Cart)
        {
            currentShopper.GetCart().PlaceItemInShoppingCart(this);
            return;
        }
        currentShopper.TryPickupItem(this);
        DisableItemRenderer();

    }

    public void DisableItemRenderer()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public ShelfItem CreateItemClone()
    {
        DisableItemRenderer();
        GameObject itemClone = Instantiate(data.prefab, transform.position, Quaternion.identity);
        itemClone.GetComponent<Collider>().enabled = false;
        return itemClone.GetComponent<ShelfItem>();
        
    }

    public void VisualHintForInteractable()
    {

    }

    public void EnablePhysics()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    public ShelfItemData GetItemData()
    {
        return data;
    }
}
