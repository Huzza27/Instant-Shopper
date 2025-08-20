using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] private float sensitivity = 200f;
    [SerializeField] private Transform playerBody;
    [SerializeField] private bool smoothMouseInput = true;
    [SerializeField] private float smoothingSpeed = 15f;
    
    private float xRot;
    private float smoothMouseX;
    private float smoothMouseY;
    
    void Start() 
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        
        if (smoothMouseInput)
        {
            // Smooth mouse input to reduce jitter
            smoothMouseX = Mathf.Lerp(smoothMouseX, mouseX, Time.deltaTime * smoothingSpeed);
            smoothMouseY = Mathf.Lerp(smoothMouseY, mouseY, Time.deltaTime * smoothingSpeed);
            
            mouseX = smoothMouseX;
            mouseY = smoothMouseY;
        }
        
        // Apply sensitivity and time scaling
        mouseX *= sensitivity * Time.deltaTime;
        mouseY *= sensitivity * Time.deltaTime;
        
        // Vertical look (camera pitch)
        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -89f, 89f);
        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
        
        // Horizontal look (player body yaw)
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
    
    // Method to reset camera smoothing (useful when teleporting or state changes)
    public void ResetSmoothing()
    {
        smoothMouseX = 0f;
        smoothMouseY = 0f;
    }
    
    // Method to temporarily disable/enable camera movement
    public void SetCameraEnabled(bool enabled)
    {
        this.enabled = enabled;
        if (!enabled)
        {
            ResetSmoothing();
        }
    }
}