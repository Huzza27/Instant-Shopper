using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public enum NPCState
{
    Idle,
    Shopping,
    Ragdoll
}

public class ShopperNPC : MonoBehaviour, IShopperNPC, ICartCollidable, IDriver
{
    private NPCState currentState = NPCState.Idle;
    public float wanderRadius = 10f;
    public float itemCollectionDelay;
    public float moveToNewShelfDelay;
    [SerializeField] private NavMeshAgent agent;
    private NavMeshAgent tempNPCAgent;
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
    [SerializeField] private Transform lastNavMeshTarget;
    [SerializeField] private bool isStunned = false;
    [SerializeField] private Transform neckTransform, stunLookAtTarget;
    [SerializeField] private float minForceForStun;
    [SerializeField] private GameObject stunUI;
    [SerializeField] private GameObject mainAvatarObject;
    [SerializeField] UnityEvent<bool> OnShopperStun;
    public float rotationSmoothTime = 0.1f; // smaller = snappier
    public float positionSmoothTime = 0.05f;
    private float rotationVelocity;

    [SerializeField] private UnityEvent<Vector3> OnForceRagdoll;

    bool isDrivingCart = false;
    public Transform GetLastNavMeshAgentTarget() => lastNavMeshTarget;
    public bool GetIsStunned() => isStunned;
    [SerializeField] private float delayForStun = 7f;

    private IDriveable currentDriveable;

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
    private void Update()
    {
        if (currentState == NPCState.Ragdoll)
        {
            HandleRootObjectFolowForRagdoll();
        }
        HandleCartMovement();
    }

    void HandleRootObjectFolowForRagdoll()
    {
        if (Vector3.Distance(transform.position, ragDollObject.transform.position) < 0.1f)
        {
            return;
        }
        transform.position = ragDollObject.transform.position;
    }

    public void HandleCartMovement()
    {
        if (isDrivingCart)
        {
            // --- Position ---
            Vector3 targetPos = new Vector3(
                cart.GetStandingPoint().position.x,
                transform.position.y,
                cart.GetStandingPoint().position.z
            );

            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime / positionSmoothTime);

            // --- Rotation ---
            float targetY = cart.GetCartTransform().eulerAngles.y;
            float newY = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetY,
                ref rotationVelocity,
                rotationSmoothTime
            );

            transform.rotation = Quaternion.Euler(0f, newY, 0f);
        }
    }


    private void LateUpdate()
    {
        if (isStunned)
        {
            LookAtTarget(stunLookAtTarget);
        }
    }

    public void LookAtTarget(Transform target)
    {
        if (target == null) return;
        Vector3 dir = target.position;
        neckTransform.LookAt(dir);
    }

    #region CART MOUNTING
    public void FindAndMountCart(System.Action onComplete)
    {
        if (!isAgentActive())
        {
            agent.enabled = true;
        }
        cart = cartObj.GetComponent<ICart>(); //Test case
        StartCoroutine(MoveToCart(() =>
        {
            MountCart(cart, onComplete);
            SetDriverForCart();
        }));
    }

    public bool isAgentActive()
    {
        return agent.enabled;
    }

    public void SetDriverForCart()
    {
        var playerCart = cart as PlayerCart;
        if (playerCart != null)
        {
            playerCart.SetDriver(this as IDriver);
        }
    }

    public IEnumerator MoveToCart(System.Action onArrive)
    {
        Vector3 target = cart.GetStandingPoint().position;
        float threshold = 0.5f; // tweak as needed

        // If already close enough, skip moving
        if (Vector3.Distance(transform.position, target) <= threshold)
        {
            Debug.Log("Already at cart, skipping movement");
            onArrive?.Invoke();
            yield break;
        }

        agent.SetDestination(target);
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
        SetDrivable(cart as IDriveable);
        agent.enabled = false;
        transform.localRotation = Quaternion.identity;
        MoveHandsToCartHandles();
        currentDriveable.GetNavMeshAgent().enabled = true;
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
            lastNavMeshTarget = slot.GetAIShelfTransform();
            MoveToNewPosition(slot.GetAIShelfTransform());

            // Wait until the agent arrives
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance ||
                Vector3.Distance(transform.position, slot.GetAIShelfTransform().position) > agent.stoppingDistance)
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

    public void StunNPC(GameObject objCollidedWith)
    {
        return;
        if (currentState == NPCState.Idle)
        {
            Debug.Log("NPC Stunned");
            isStunned = true;
            animator.SetBool("isStunned", true);
            //Look at player somehow
            Invoke(nameof(ReEnableNPC), delayForStun);
        }
    }

    public void ToggleAlertUI()
    {
        stunUI.SetActive(!stunUI.activeSelf);
    }

    Transform GetShopperFromCartCollision(GameObject objCollidedWith)
    {
        GameObject rootObj = objCollidedWith.transform.root.gameObject;
        PlayerCart cart = rootObj.GetComponent<PlayerCart>();
        Shopper player = cart.GetDriver() as Shopper;
        if (player == null)
        {
            return null;
        }
        return player.gameObject.transform;
    }

    public void ReEnableNPC()
    {

        ToggleAlertUI();
        Debug.Log("NPC Returning To Cart");
        StartCoroutine(MoveToCart(() =>
        {
            animator.SetBool("isStunned", false);
            (cart as PlayerCart).ReEnableAgentAfterStun(this);
        }));
    }

    public bool IsEnoughForceToBeStunned(Vector3 force)
    {
        if (force.magnitude >= minForceForStun)
        {
            return true;
        }
        return false;
    }

    #region RAGDOLLING AND COLLISIONS

    public void StaticRagdoll()
    {
        SetState(NPCState.Ragdoll);
        agent.isStopped = true;
        animator.enabled = false;
        ragDollObject.SetActive(true);
        SecurityAlertManager.Instance.AlertSecurity(transform.position);
        Invoke(nameof(GetUpAfterRagDoll), 5f); // Automatically unragdoll after 5 seconds
    }

    public void UnRagdoll()
    {
        SetState(NPCState.Idle);
        ToggleCartRB();
        //StartCoroutine(ShopRoutine());
    }

    public void GetUpAfterRagDoll()
    {
        //WHEN I HAVE GET UP ANIMATION SHIT, PUT IT HERE
        SetState(NPCState.Idle);        
    }

    public void ForceRagdoll(Vector3 force)
    {
        if (isDrivingCart)
        {
            UpdateCartFollowFlag(false);
        }
        SetState(NPCState.Ragdoll);
        OnForceRagdoll?.Invoke(force);
    }

    public void UpdateCartFollowFlag(bool value)
    {
        isDrivingCart = value;
    }


    public void MoveHandsToCartHandles()
    {
        LeanTween.value(0, 1, 0.5f).setOnUpdate((float val) =>
        {
            rig.weight = val;
        });
    }

    public void OnCartCollision(Vector3 impactForce, GameObject objCollidedWith)
    {
        ToggleCartRB();
        ForceRagdoll(impactForce);
    }

    void ToggleCartRB()
    {
        if (cart != null)
        {
            var playerCart = cart as PlayerCart;
            if (playerCart != null)
            {
                playerCart.ToggleRigidbody();
            }
        }
    }

    #endregion

    public bool IsPlayer()
    {
        return false;
    }

    public GameObject GetObject()
    {
        return this.gameObject;
    }

    public void SetDrivable(IDriveable driveable)
    {
        currentDriveable = driveable;
    }
    
    public IDriveable GetDrivable()
    {
        return currentDriveable;
    }
}
