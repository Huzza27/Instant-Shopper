using Unity.VisualScripting;
using UnityEngine;

public class RagDollBufferCollider : MonoBehaviour
{
    [SerializeField] private PlayerCart playerCart;
    [SerializeField] private float velocityThreshold = 0.5f;

    void OnTriggerEnter(Collider other)
    {
        
        float speed = playerCart.GetCartVelocity();

        if (speed > velocityThreshold && other.transform.root.TryGetComponent<ICartCollidable>(out var collidable))
        {
            // Direction from cart to NPC (push away)
            Vector3 dir = (other.transform.position - playerCart.transform.position).normalized;

            // Scale by cart velocity for impact force
            Vector3 impactForce = dir * speed;
            Debug.Log(gameObject.name + "collided with " + other.gameObject.name);
            collidable.OnCartCollision(impactForce, playerCart.gameObject);
        }
    }
}
