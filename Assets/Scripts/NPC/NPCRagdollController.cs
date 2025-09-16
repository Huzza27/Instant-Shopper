using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class NPCRagdollController : MonoBehaviour
{
    [SerializeField] GameObject npcPhysicsSwitchGO;
    [SerializeField] Collider mainCollider;
    [SerializeField] Animator npcAnimator;
    [SerializeField] GameObject originalHipsParent;
    [SerializeField] float hipsLocalYOffset = 1.080262f;
    [SerializeField] ShopperNPC npcScript;
    [SerializeField] NavMeshAgent npcAgent;
    [SerializeField] float knockoutDuration;
    [SerializeField] UnityEvent OnGetUp;
    public void RagdollNPCWithForce(Vector3 force)
    {
        //NOT WORKING BECAUSE I AM NOT TURING OFF PLAYER OR CART NAVMESH
        IDriveable driveable = npcScript.GetDrivable();
        if (driveable != null)
        {
            ToggleNavAgent(driveable.GetNavMeshAgent());
        }
        ToggleMainCollider();
        ToggleAnimator();
        SwitchToPhysics();
        DistributeForce(force);
        Invoke(nameof(GetUpAfterRagDoll), knockoutDuration);  
    }

    public void GetUpAfterRagDoll()
    {
        OnGetUp?.Invoke();
        ToggleAnimator();
        HandleFullHipsReset();
        ToggleMainCollider();
        npcScript.FindAndMountCart(null);
    }


    #region GETTING UP HELPER FUNCTIONS
    public void HandleFullHipsReset()
    {
        SwitchToKinematic(); 
    }

    public void SwitchToKinematic()
    {
        npcPhysicsSwitchGO.transform.SetParent(originalHipsParent.transform, true);
        //npcPhysicsSwitchGO.transform.localPosition = new Vector3(0, hipsLocalYOffset, 0);
        npcPhysicsSwitchGO.SetActive(false);
    }


    #endregion

    #region RAGDOLLING HELPER FUNCTIONS

    void ToggleMainCollider()
    {
        mainCollider.enabled = !mainCollider.enabled;
    }

    void ToggleAnimator()
    {
        npcAnimator.enabled = !npcAnimator.enabled;
    }

    void SwitchToPhysics()
    {
        npcPhysicsSwitchGO.transform.parent = null;
        npcPhysicsSwitchGO.SetActive(true);
    }

    void DistributeForce(Vector3 force)
    {
        npcPhysicsSwitchGO.GetComponent<Rigidbody>().AddForce(force * 60, ForceMode.Impulse);
    }

    void ToggleNavAgent(NavMeshAgent agent)
    {
        agent.enabled = !agent.enabled;
    }
    #endregion
}
