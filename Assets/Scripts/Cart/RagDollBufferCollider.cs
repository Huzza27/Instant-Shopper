using UnityEngine;

public class RagDollBufferCollider : MonoBehaviour
{
    [SerializeField] private PlayerCart playerCart;
    [SerializeField] private float velocityThreshold = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        float speed = playerCart.GetCartVelocity();

        if (speed > velocityThreshold && other.CompareTag("NPC"))
        {
            // Direction from cart to NPC (push away)
            Vector3 dir = (other.transform.position - playerCart.transform.position).normalized;

            // Scale by cart velocity for impact force
            Vector3 impactForce = dir * speed;

            other.transform.root.GetComponent<IShopperNPC>()?.ForceRagdoll(impactForce);
        }
    }
}
