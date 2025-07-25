using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;     // Rotates with yaw
    [SerializeField] private Transform cameraTransform; // Child of pivot; rotates with pitch
    [SerializeField] private ConfigurableJoint playerJoint;

    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float clampAngle = 80f;

    [Header("Driving")]
    [SerializeField] private Transform cartTransform;  // Assign the cart root transform
    public bool isDriving = false;                     // Toggle this externally when player enters/exits cart

    private float pitch = 0f;
    private float yaw = 0f;
    private float mouseX;
    private float mouseY;

    private float fixedYaw;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yaw = cameraPivot.eulerAngles.y;

        playerJoint.axis = Vector3.right;
        playerJoint.secondaryAxis = Vector3.up;
    }

    void Update()
    {
        if (!isDriving)
        {
            mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -clampAngle, clampAngle);
        }
    }

    void LateUpdate()
    {
        if (isDriving && cartTransform != null)
        {
            // While driving, sync rotation with the cart
            float cartYaw = cartTransform.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, cartYaw, 0f);
        }
        else
        {
            // Normal camera control
            yaw += mouseX;
            yaw = Mathf.Repeat(yaw, 360f);

            cameraPivot.rotation = Quaternion.Euler(0f, yaw, 0f);
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            playerJoint.targetRotation = Quaternion.Euler(0f, -yaw, 0f);

            CacheFixedYaw();
        }
    }

    public void CacheFixedYaw()
    {
        fixedYaw = yaw;
    }

    public float GetCachedYaw()
    {
        return fixedYaw;
    }
}
