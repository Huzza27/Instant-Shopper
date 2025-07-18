using UnityEngine;

public class WobblyMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float deceleration = 8f;

    public Transform cartHandleTransform;
    [SerializeField] private float turnSpeed = 120f;

    private Rigidbody rb;
    private Vector3 targetVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!GetIsUsingCart())
        {
            HandleRegularMovement();
        }
        else
        {
            HandleCartMovement();
        }
    }

    public void HandleCartMovement()
    {
        float h = Input.GetAxisRaw("Horizontal"); // A/D for turning
        float v = Input.GetAxisRaw("Vertical");   // W/S for forward/backward

        // --- Turning (A/D) ---
        float turnAmount = h * turnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // --- Forward/Backward (W/S) ---
        Vector3 moveDirection = transform.forward * v * moveSpeed;
        Vector3 velocity = new Vector3(moveDirection.x, rb.linearVelocity.y, moveDirection.z);
        rb.linearVelocity = velocity;
    }

    public bool GetIsUsingCart()
    {
        return PlayerStateManager.Instance.GetPlayerState() == PlayerState.Cart;
    }

    public void HandleRegularMovement()
    {
        // Get input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = (transform.forward * v + transform.right * h).normalized;
        targetVelocity = inputDir * moveSpeed;

        // Smoothly move toward the target velocity
        Vector3 velocityChange = targetVelocity - rb.linearVelocity;

        // Split acceleration and deceleration for tighter control
        float accelRate = (targetVelocity.magnitude > 0.1f) ? acceleration : deceleration;

        velocityChange = Vector3.ClampMagnitude(velocityChange, accelRate);

        // Maintain current vertical velocity (no gravity override)
        velocityChange.y = 0f;

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}
