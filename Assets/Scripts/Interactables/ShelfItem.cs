using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Flexalon;

public class ShelfItem : MonoBehaviour, IInteractable, IPlaceableInCart, IThrowable
{
    private Rigidbody rb;
    [SerializeField] private ShelfItemData originalItemData;
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
            ICart cart = currentShopper.GetDriveable() as ICart;
            cart.PlaceItemInShoppingCart(this);
            return;
        }
        currentShopper.TryPickupItem(this);
        //DisableItemRenderer();
    }

    public void DisableItemRenderer()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }


    public ShelfItem CreateItemClone()
    {
        //DisableItemRenderer();
        GameObject itemClone = Instantiate(data.prefab, transform.position, Quaternion.identity);
        //itemClone.GetComponent<Collider>().enabled = false;
        return itemClone.GetComponent<ShelfItem>();
    }

    public void VisualHintForInteractable()
    {
        // Optional
    }

    public void SetOriginalItemDataReference(ShelfItemData newData)
    {
        originalItemData = newData;
        Debug.Log(this.gameObject + "'s original item data is " + data);
    }

    public ShelfItemData GetOriginalItemDataReferece()
    {
        return originalItemData;
    }

    public void SetItemData(ShelfItemData itemData)
    {
        data = itemData;
    }

    public ShelfItemData GetItemData()
    {
        return data;
    }

    public void OnPlacedIntoCart(Transform cart)
    {
        //gameObject.AddComponent<FlexalonObject>();
    }

    public void ThrowItem(float throwForce)
    {
        Rigidbody rb = TryCreateRBForThrowing();
        TryEnableCollider();
        rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
    }

    void TryEnableCollider()
    {
        Collider collider = GetComponent<Collider>();
        if (collider.isTrigger)
        {
            collider.isTrigger = false;
        }
    }

    Rigidbody TryCreateRBForThrowing()
    {
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        return rb;
    }

    public Vector3 GetThrowVelocity()
    {
        return rb != null ? rb.linearVelocity * rb.mass : Vector3.zero;
    }
}
