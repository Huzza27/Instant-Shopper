using UnityEngine;
using UnityEngine.Events;

public class RagDollColldier : MonoBehaviour
{
    [SerializeField] private UnityEvent OnRagdollCollision;
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IThrowable throwable))
        {
            OnRagdollCollision?.Invoke();
        }
    }
}
