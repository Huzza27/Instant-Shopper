using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerCart : MonoBehaviour, ICart, IDriveable, IInteractable
{
    [SerializeField] private CartItemPlacer itemPlacer;
    [SerializeField] private Transform itemGrid;
    [SerializeField] private Transform playerStandingPoint;
    [SerializeField] private Transform cartCameraMount;
    [SerializeField] Transform rotationPivot;
    [SerializeField] private Rigidbody cartRB;
    [SerializeField] CartMovement motorScript;

    [SerializeField] private float cameraTransitionDuration;

    public Transform GetDropTransform() => itemGrid;
    public Transform GetCartTransform() => this.transform;

    public Transform GetCameraMount() => cartCameraMount;
    public Transform GetStandingPoint() => playerStandingPoint;
    public Transform GetRotationPivot() => rotationPivot;
    public Rigidbody GetCartRB() => cartRB;
    [SerializeField] private List<ShelfItem> cartIventory = new List<ShelfItem>();

    public void OnItemPlaced(GameObject item)
    {
        item.GetComponent<IPlaceableInCart>().OnPlacedIntoCart(this.transform);
        cartIventory.Add(item.GetComponent<ShelfItem>());
    }
    public void PlaceItemInShoppingCart(ShelfItem item)
    {
        itemPlacer.PlaceInCart(item, this);
    }

    public List<ShelfItem> GetItems()
    {
        return cartIventory;
    }

    public void ClearCartInventory()
    {
        foreach (ShelfItem item in cartIventory)
        {
            Destroy(item.gameObject); // Destroy the item GameObject
        }
        cartIventory.Clear();
    }



    public void Mount(Shopper shopper)
    {
        shopper.SetDrivable(this);
        motorScript.SetDriver(shopper);
        MountCamera(shopper);
    }


    public void MountPlayer(Shopper shopper)
    {
        shopper.OnSetMovementTarget?.Invoke(playerStandingPoint); //Sets the movement target in PlayerMovement
        PlayerStateManager.Instance.SetMovementMode(MovementMode.Targeted);
        shopper.OnMountDriveable?.Invoke(this);
    }

    void MountCamera(Shopper shopper)
    {
        CameraTransitionManager.Instance.AnimateCameraToPosition(cartCameraMount, cameraTransitionDuration,() =>
        {
            MountPlayer(shopper);
        });
    }

    public void Dismount(Shopper shopper)
    {
        throw new System.NotImplementedException();
    }

    public void Interact(InteractionContexzt context, Shopper currentShopper)
    {
        if (!currentShopper.GetIsPlayerHoldingSomething())
        {
            PlayerStateManager.Instance.SetPlayerState(PlayerState.Cart);
            Mount(currentShopper);
            EnableMotor();
        }
    }

    public void VisualHintForInteractable()
    {
        throw new System.NotImplementedException();
    }

    public Transform GetSeatTransform()
    {
        return playerStandingPoint; // Assuming the player stands in front of the cart
    }

    public void EnableMotor()
    {
        if (motorScript.enabled == false)
        {
            motorScript.enabled = true;
        }
    }
}
