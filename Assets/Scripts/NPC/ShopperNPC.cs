using UnityEngine;
using UnityEngine.AI;

public class ShopperNPC : MonoBehaviour, IShopperNPC
{
    public Transform[] shoppingSpots; // Assign shelves in the inspector
    public float idleTime = 2f;

    private NavMeshAgent agent;
    private float idleTimer;
    private Transform targetSpot;
    private enum State { Wandering, GoingToShelf, Shopping }
    private State currentState;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject ragDollObject;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        idleTimer = idleTime;
        PickNewShelf();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.GoingToShelf:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    idleTimer -= Time.deltaTime;
                    if (idleTimer <= 0f)
                    {
                        PickNewShelf();
                    }
                }
                break;
        }
    }

    void PickNewShelf()
    {
        if (shoppingSpots.Length == 0) return;

        targetSpot = shoppingSpots[Random.Range(0, shoppingSpots.Length)];
        agent.SetDestination(targetSpot.position);
        idleTimer = idleTime;
        currentState = State.GoingToShelf;
    }

    public void Ragdoll()
    {
        animator.enabled = false;
        ragDollObject.SetActive(true);
        ragDollObject.GetComponent<Rigidbody>().AddForce(Vector3.one * 10f, ForceMode.Impulse);
    }

    public void UnRagdoll()
    {
        throw new System.NotImplementedException();
    }
}
