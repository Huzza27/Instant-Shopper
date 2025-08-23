using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CartMovement : MonoBehaviour
{
    [SerializeField] PlayerCart currentCart;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float turnSpeed = 180f;
    [SerializeField] Shopper driver;
    [SerializeField] InputAction rightMouseDown;
    [SerializeField] float cameraResetSpeed = 5f;
    [SerializeField] float alignmentForce = 2f; // How strongly cart follows camera
    [SerializeField] float maxAlignmentAngle = 45f; // Max angle before forced alignment
    
    private bool cartCanRotate = true;
    private bool isResettingCamera = false;
    
    // Cached inputs - now updated in FixedUpdate for consistency
    private float inputMouseX;
    private float inputV;
    private float smoothMouseX; // Smoothed mouse input to reduce jitter
    
    // Camera alignment tracking
    private float cameraCartAngleDifference;
    
    void Start()
    {
        rightMouseDown.Enable();
        rightMouseDown.performed += _ => SetRotationFlagForCart(false);
        rightMouseDown.canceled += _ => SetRotationFlagForCart(true);
    }
    
    public void SetDriver(Shopper newDriver)
    {
        driver = newDriver;
    }
    
    void Update()
    {
        if (!currentCart) return;
        
        // Handle camera reset animation
        if (isResettingCamera && currentCart != null)
        {
            var mount = currentCart.GetCameraMount();
            Quaternion targetRotation = Quaternion.identity;
            mount.localRotation = Quaternion.Slerp(
                mount.localRotation,
                targetRotation,
                Time.deltaTime * cameraResetSpeed
            );
            
            if (Quaternion.Angle(mount.localRotation, targetRotation) < 0.1f)
            {
                mount.localRotation = targetRotation;
                isResettingCamera = false;
            }
        }
    }
    
    void FixedUpdate()
    {
        if (!currentCart) return;
        
        // Update inputs in FixedUpdate for physics consistency
        inputMouseX = Input.GetAxis("Mouse X");
        inputV = Input.GetAxis("Vertical");
        
        // Smooth mouse input to reduce jitter
        smoothMouseX = Mathf.Lerp(smoothMouseX, inputMouseX, Time.fixedDeltaTime * 10f);

        // Calculate angle difference between camera and cart
        if (cartCanRotate)
        {
            UpdateCameraCartAlignment();
        }
        HandleCartDriving();
    }
    
    private void UpdateCameraCartAlignment()
    {
        if (!driver) return;
        
        // Get the camera's forward direction (driver's forward)
        Vector3 cameraForward = driver.transform.forward;
        Vector3 cartForward = currentCart.transform.forward;
        
        // Calculate signed angle between camera and cart
        cameraCartAngleDifference = Vector3.SignedAngle(cartForward, cameraForward, Vector3.up);
    }
    
    private void HandleCartDriving()
    {
        Rigidbody rb = currentCart.GetCartRB();
        
        if (cartCanRotate)
        {
            // Cart rotation mode
            HandleCartRotation(rb);
        }
        else
        {
            // Camera look mode
            HandleCameraRotation();
        }
        
        // Handle movement
        HandleCartMovement(rb);
    }
    
    private void HandleCartRotation(Rigidbody rb)
    {
        float rotationInput = 0f;
        
        // Primary rotation from mouse input
        if (Mathf.Abs(smoothMouseX) > 0.001f)
        {
            rotationInput = smoothMouseX * turnSpeed;
        }
        
        // Add alignment force when camera and cart are misaligned
        if (Mathf.Abs(cameraCartAngleDifference) > 5f) // Small dead zone
        {
            float alignmentInput = cameraCartAngleDifference * alignmentForce;
            rotationInput += alignmentInput;
        }
        
        // Force alignment if angle gets too large
        if (Mathf.Abs(cameraCartAngleDifference) > maxAlignmentAngle)
        {
            float forceAlignmentInput = cameraCartAngleDifference * alignmentForce * 3f;
            rotationInput = forceAlignmentInput; // Override other inputs
        }
        
        // Apply rotation
        if (Mathf.Abs(rotationInput) > 0.001f)
        {
            float angularVelocity = rotationInput * Time.fixedDeltaTime;
            Quaternion deltaRotation = Quaternion.Euler(0f, angularVelocity, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
    }
    
    private void HandleCameraRotation()
    {
        // When not rotating cart, apply mouse input to camera mount
        if (Mathf.Abs(inputMouseX) > 0.001f)
        {
            Transform cameraMount = currentCart.GetCameraMount();
            float cameraRotation = inputMouseX * turnSpeed * Time.fixedDeltaTime;
            cameraMount.localRotation *= Quaternion.Euler(0f, cameraRotation, 0f);
        }
    }
    
    private void HandleCartMovement(Rigidbody rb)
    {
        if (Mathf.Abs(inputV) > 0.001f)
        {
            Vector3 moveDirection = currentCart.transform.forward * inputV;
            Vector3 moveVelocity = moveDirection * moveSpeed;
            
            // Use velocity for smoother movement
            rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);
        }
        else
        {
            // Stop horizontal movement when no input
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }
    
    void SetRotationFlagForCart(bool canRotate)
    {
        cartCanRotate = canRotate;
        if (canRotate) 
        {
            isResettingCamera = true;
        }
    }
    
    // Optional: Debug visualization
    void OnDrawGizmos()
    {
        if (!currentCart || !driver) return;
        
        // Draw cart forward direction in blue
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(currentCart.transform.position, currentCart.transform.forward * 2f);
        
        // Draw camera forward direction in red
        Gizmos.color = Color.red;
        Gizmos.DrawRay(currentCart.transform.position + Vector3.up * 0.5f, driver.transform.forward * 2f);
    }
}