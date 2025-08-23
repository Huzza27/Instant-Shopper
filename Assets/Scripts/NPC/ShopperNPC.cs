using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public enum NPCState
{
    Idle,
    Shopping,
    Ragdoll
}

public class ShopperNPC : MonoBehaviour, IShopperNPC
{
    private NPCState currentState = NPCState.Idle;
public float wanderRadius = 10f;
    public float itemCollectionDelay;
    public float moveToNewShelfDelay;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private int numItemsToCollect;
    private int itemsCollected = 0;
    [SerializeField] private GameObject cartObj;
    private ICart cart;
    private List<ShelfItem> shopperInventory = new List<ShelfItem>();
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject ragDollObject;
    [SerializeField] private Collider mainCollider;
    [SerializeField] private float cartImpactForceMultiplier = 1f;
    [SerializeField] private Rig rig;
    bool isDrivingCart = false;

    void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        FindAndMountCart(() =>
        {
            isDrivingCart = true;
            StartCoroutine(ShopRoutine());
        });
    }
    private void Update() {
        if (isDrivingCart)
        {
            Vector3 move = new Vector3(cart.GetStandingPoint().position.x, transform.position.y, cart.GetStandingPoint().position.z);
            transform.position = move;
            transform.rotation = cart.GetCartTransform().rotation;
        }
    }

    #region CART
    void FindAndMountCart(System.Action onComplete)
    {
        cart = cartObj.GetComponent<ICart>(); //Test case
        StartCoroutine(MoveToCart(() =>
        {
            MountCart(cart, onComplete);
        }));
    }

    public IEnumerator MoveToCart(System.Action onArrive)
    {
        agent.SetDestination(cart.GetStandingPoint().position);
        Debug.Log("Path to cart set");
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            Debug.Log("Moving to cart...");
            yield return null;
        }
        Debug.Log("Arrived at cart");
        onArrive?.Invoke();
    }


    public void MountCart(ICart cart, System.Action onComplete)
    {
        this.cart = cart;
        agent.enabled = false;
        transform.localRotation = Quaternion.identity;
        MoveHandsToCartHandles();
        agent = cart.GetNavMeshAgent();
        agent.transform.SetParent(null);
        cartObj.GetComponent<PlayerCart>().SetNPCDriverFlag(true);
        onComplete?.Invoke(); //Starts shopping routine in Initialize
    }
    #endregion

    #region SHOPPING
    private IEnumerator ShopRoutine()
    {
        if (currentState == NPCState.Ragdoll)
        {
            yield break;
        }
        while (itemsCollected < numItemsToCollect)
        {
            Shelf shelf = GetShelf();
            ShelfSlot slot = GetNewSlot(shelf);
            MoveToNewPosition(slot.GetAIShelfTransform());

            // Wait until the agent arrives
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            // Now collect items
            CollectItemsOffShelf(slot);
            // Wait a short time before going to next shelf (optional)
            yield return new WaitForSeconds(moveToNewShelfDelay);
        }
        OnShoppingComplete();


    }

    public void OnShoppingComplete()
    {
        ICashRegister register = CashRegisterManager.Instance.GetRandomRegister();
        MoveToNewPosition(register.GetRegisterPoisiton());
    }

    public void TestDestyroyNPC()
    {
        Destroy(this.gameObject);
    }

    void PlaceItemInCart(ShelfItem item)
    {
        cart.PlaceItemInShoppingCart(item);
    }

    public void MoveToNewPosition(Transform targetPosition)
    {
        agent.SetDestination(targetPosition.position);
    }

    public void CollectItemsOffShelf(ShelfSlot slot)
    {
        List<ShelfItem> itemsInSlot = slot.GetItemsInSlot();
        int maxItemsCanTake = Mathf.Min(itemsInSlot.Count, numItemsToCollect - itemsCollected);

        if (maxItemsCanTake <= 0)
            return;

        int itemsToCollect = Random.Range(1, maxItemsCanTake + 1); // Always at least 1

        for (int i = 0; i < itemsToCollect; i++)
        {
            ShelfItem item = itemsInSlot[0];
            StartCoroutine(CollectItem(slot, item));
            Debug.Log($"Collected item: {item.name}. Total collected: {itemsCollected}");

            if (itemsCollected >= numItemsToCollect)
                break;
        }
    }

    IEnumerator CollectItem(ShelfSlot slot, ShelfItem item)
    {
        yield return new WaitForSeconds(itemCollectionDelay);
        slot.RemoveItemFromSlot();
        shopperInventory.Add(item);
        PlaceItemInCart(item);
        itemsCollected++;
    }


    public ShelfSlot GetNewSlot(Shelf shelf)
    {
        ShelfSlot randomSlot = shelf.GetRandomSlot();
        Debug.Assert(randomSlot != null, "No slots available in the shelf.");
        return randomSlot;
    }


    Shelf GetShelf()
    {
        Shelf shelf = ShelfManager.Instance.GetRandomShelf();
        Debug.Assert(shelf != null, "No shelves available in ShelfManager.");
        return shelf;

    }
    #endregion

   

    public void SetState(NPCState state)
    {
        currentState = state;
    }
    public void ReactToChaos()
    {
        throw new System.NotImplementedException();
    }



    public void UnRagdoll()
    {
        agent.isStopped = false;
        ragDollObject.SetActive(false);
        animator.enabled = true;
        mainCollider.enabled = true;
        SetState(NPCState.Idle);
        //StartCoroutine(ShopRoutine());
    }

    public void StaticRagdoll()
    {
        SetState(NPCState.Ragdoll);
        agent.isStopped = true;
        animator.enabled = false;
        ragDollObject.SetActive(true);
        SecurityAlertManager.Instance.AlertSecurity(transform.position);
        Invoke(nameof(UnRagdoll), 5f); // Automatically unragdoll after 5 seconds
    }

    public void ForceRagdoll(Vector3 force)
    {
        SetState(NPCState.Ragdoll);
        mainCollider.enabled = false;
        agent.isStopped = true;
        animator.enabled = false;
        ragDollObject.SetActive(true);
        ragDollObject.GetComponent<Rigidbody>().AddForce(force * cartImpactForceMultiplier, ForceMode.Impulse);
        SecurityAlertManager.Instance.AlertSecurity(transform.position);
        Invoke(nameof(UnRagdoll), 5f); // Automatically unragdoll after 5 seconds
    }


    public void MoveHandsToCartHandles()
    {
        LeanTween.value(0, 1, 0.5f).setOnUpdate((float val) =>
        {
            rig.weight = val;
        });
    }
}
