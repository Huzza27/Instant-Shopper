using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class ShelfItem : MonoBehaviour, IInteractable, IPlaceableInCart
{
    private Rigidbody rb;
    [SerializeField] ShelfItemData data;
    [SerializeField] Vector3 cloneScale;
    [SerializeField] private float settleDelay = 1.5f; // Delay before settling the item in the cart

    private void Awake()
    {
        data = GetComponent<ShelfItemData>();
        GetComponent<Renderer>().shadowCastingMode = ShadowCastingMode.Off;
    }


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
        // Optional
    }

    public ShelfItemData GetItemData()
    {
        return data;
    }

    public void OnPlacedIntoCart(Transform cart)
    {
        return;
    }
}
