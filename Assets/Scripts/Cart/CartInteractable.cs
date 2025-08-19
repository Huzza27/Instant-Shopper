using Unity.Mathematics;
using UnityEngine;

public class CartInteractable : MonoBehaviour, IInteractable{
    public Transform locationWherePlayerMovesToPushCart;
    [SerializeField] Rigidbody rb;
    [SerializeField] private ICart cart;
    void Start()
    {
        cart = this.GetComponent<ICart>();
        if (cart == null)
        {
            Debug.LogError("CartInteractable requires an ICart component to function properly.");
        }
    }

    public void Interact(InteractionContexzt context, Shopper currentShopper)
    {

        if (!currentShopper.GetIsPlayerHoldingSomething())
        {
            //rb.MovePosition(context.cartHandlePoint.position);
            currentShopper.OnShoppingCartInteract?.Invoke(cart);
            //currentShopper.SetCart(cart);
            PlayerStateManager.Instance.SetPlayerState(PlayerState.Cart);
        }
        else
        {
            ShelfItem item = currentShopper.GetHeldItem();
            cart.PlaceItemInShoppingCart(item);
            currentShopper.RemoveItemFromInventory(item);
        }

    }
    public void VisualHintForInteractable()
    {
        throw new System.NotImplementedException();
    }
}
