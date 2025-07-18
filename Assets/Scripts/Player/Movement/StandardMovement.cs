using UnityEngine;

public class StandardMovement : BaseMovementBehavior
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 8f;

    private Vector3 targetVelocity;


    void FixedUpdate()
    {
        Move(GetComponent<Rigidbody>());
    }

    /*HEY! Before you add ANY multiplayer networking code to you game. You are going to do this first
    -Create Player Inputmanager
    -Divide animation syncing between two classes
    thank you for your time :)
    */
    public override void Move(Rigidbody rb)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");


        Vector3 inputDir = (rb.transform.forward * v + rb.transform.right * h).normalized;
        targetVelocity = inputDir * moveSpeed;

        Vector3 velocityChange = targetVelocity - rb.linearVelocity;
        float accelRate = (targetVelocity.magnitude > 0.1f) ? acceleration : deceleration;
        velocityChange = Vector3.ClampMagnitude(velocityChange, accelRate);
        velocityChange.y = 0f;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}
