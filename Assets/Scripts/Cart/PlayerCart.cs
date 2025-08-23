using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
public class PlayerCart : MonoBehaviour, ICart, IDriveable, IInteractable
{
    [SerializeField] private bool isBeingDriven;
    [Header("PLAYER")]
    [SerializeField] private CartItemPlacer itemPlacer;
    [SerializeField] private Transform itemGrid;
    [SerializeField] private Transform playerStandingPoint;
    [SerializeField] private Transform cartCameraMount;
    [SerializeField] Transform rotationPivot;
    [SerializeField] private Rigidbody cartRB;
    [SerializeField] CartMovement motorScript;
    [Header("NPC")]
    [SerializeField] private Transform npcAgent;

    [SerializeField] private float cameraTransitionDuration;

    public Transform GetDropTransform() => itemGrid;
    public Transform GetCartTransform() => this.transform;

    public Transform GetCameraMount() => cartCameraMount;
    public Transform GetStandingPoint() => playerStandingPoint;
    public Transform GetRotationPivot() => rotationPivot;
    public NavMeshAgent GetNavMeshAgent() => npcAgent.GetComponent<NavMeshAgent>();
    public Rigidbody GetCartRB() => cartRB;
    [SerializeField] private List<ShelfItem> cartIventory = new List<ShelfItem>();
    #region PLAYER_INTERACTION
    public void Interact(InteractionContexzt context, Shopper currentShopper)
    {
        if (!currentShopper.GetIsPlayerHoldingSomething())
        {
            PlayerStateManager.Instance.SetPlayerState(PlayerState.Cart);
            Mount(currentShopper, EnableMotor); //Enable motor is a callBack
        }
        else
        {
            PlaceItemInShoppingCart(currentShopper.GetHeldItem());
        }
    }
    #endregion

    #region PLAYER_MOUNTING_CART
    public void Mount(Shopper shopper, System.Action onComplete)
    {
        shopper.SetDrivable(this);
        motorScript.SetDriver(shopper);
        MountPlayer(shopper, () =>
        {
            PlayerStateManager.Instance.SetMovementMode(MovementMode.Targeted);
            onComplete?.Invoke();
        });
    }


    public void MountPlayer(Shopper shopper, System.Action onPlayerMountComplete = null)
    {
        shopper.OnSetMovementTarget?.Invoke(playerStandingPoint); //Sets target in PlayerMovement
        shopper.OnMountDriveable?.Invoke(this, onPlayerMountComplete); //Mount Driveable in Shopper.cs passing callBack
    }

    public void EnableMotor()
    {
        if (motorScript.enabled == false)
        {
            motorScript.enabled = true;
        }
    }


    #endregion

    #region PLAYER_DISMOUNT_CART
    public void Dismount(Shopper shopper, System.Action onDismountComplete = null)
    {
        shopper.OnSetMovementTarget?.Invoke(null); // Clear the movement target
        DisableMotor();
        PlayerStateManager.Instance.SetMovementMode(MovementMode.Default);
        onDismountComplete?.Invoke();
    }

    public void DisableMotor()
    {
        if (motorScript.enabled == true)
        {
            motorScript.enabled = false;
        }
    }


    #endregion

    #region PLAYER_ITEM_HANDLING
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
    #endregion
    public float GetCartVelocity()
    {
        return cartRB.linearVelocity.magnitude;
    }

    public void VisualHintForInteractable()
    {
        throw new System.NotImplementedException();
    }

    public Transform GetSeatTransform()
    {
        return playerStandingPoint; // Assuming the player stands in front of the cart
    }

    public void NPCMount(IShopperNPC npc)
    {
        throw new System.NotImplementedException();
    }
    
    public void SetNPCDriverFlag(bool isNPCDriver)
    {
        motorScript.enabled = true;
        motorScript.SetNPCDriverFlag();
    }

}
