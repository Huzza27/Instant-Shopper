using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NPCCart : MonoBehaviour
{
    [SerializeField] Transform testCart;
    [SerializeField] NavMeshAgent NPCAgent;
    [SerializeField] NavMeshAgent cartAgent;
    [SerializeField] public Transform NPCStandingPoint;
    [SerializeField] UnityEvent<NPCCart> OnMountCart;
    private bool hasMounted = false;

    public void TestWalkToCart()
    {
        NPCAgent.SetDestination(testCart.position);
    }

    void Awake()
    {
        TestWalkToCart();
    }


    void Update()
    {
        if (!NPCAgent.pathPending &&
            NPCAgent.remainingDistance <= NPCAgent.stoppingDistance &&
            NPCAgent.velocity.sqrMagnitude < 0.01f &&
            !hasMounted)
        {
            hasMounted = true;
            MountCart();
        }
    }


    public void MountCart()
    {
        OnMountCart?.Invoke(this);
        cartAgent.enabled = true;
    }

    
}
