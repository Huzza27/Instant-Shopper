using UnityEngine;
using UnityEngine.AI;

public class SecurityGuard : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        SecurityAlertManager.Instance?.RegisterGuard(this);
    }

    private void OnDisable()
    {
        SecurityAlertManager.Instance?.UnregisterGuard(this);
    }

    public void OnChaosReported(Vector3 chaosLocation)
    {
        agent.SetDestination(chaosLocation);
        // Optional: Add animation or state change here
        Debug.Log($"Security heading to chaos at: {chaosLocation}");
    }
}
