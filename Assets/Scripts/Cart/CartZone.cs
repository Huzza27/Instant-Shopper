using UnityEngine;
public class CartZone : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            if (rb.gameObject.CompareTag("ShelfItem")) // Optional check
            {
                rb.isKinematic = true;
            }
        }
    }
}
