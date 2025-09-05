using UnityEngine;

public class EyeballPositionMaitenence : MonoBehaviour
{
    [SerializeField] private Transform eyeSocket;
    [SerializeField] private Rigidbody rb;
    void FixedUpdate()
    {
        rb.MovePosition(eyeSocket.position); 
    }
}
