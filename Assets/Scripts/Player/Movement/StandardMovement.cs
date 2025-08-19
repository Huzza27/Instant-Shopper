using UnityEngine;

public class StandardMovement : BaseMovementBehavior
{
    public float moveSpeed = 5f;

    private CharacterController controller;

    public override void Move(Rigidbody rb)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {

    }

    public void RegularMovement()
    {
        // Get input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Move relative to player's facing direction
        Vector3 move = transform.right * h + transform.forward * v;

        // Apply movement
        controller.Move(move * moveSpeed * Time.deltaTime);
    }
}