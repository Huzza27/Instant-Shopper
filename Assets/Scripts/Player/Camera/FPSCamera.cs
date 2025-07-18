using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;     // Rotates with yaw
    [SerializeField] private Transform cameraTransform; // Child of pivot; rotates with pitch
    [SerializeField] private ConfigurableJoint playerJoint;

    [SerializeField] private float sensitivity = 100f;
    [SerializeField] private float clampAngle = 80f;

    private float pitch = 0f;
    private float yaw = 0f;

    private float mouseX;
    private float mouseY;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yaw = cameraPivot.eulerAngles.y;

        playerJoint.axis = Vector3.right;
        playerJoint.secondaryAxis = Vector3.up;
    }

    void Update()
    {
        // Read input (frame-accurate)
        mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Apply pitch
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -clampAngle, clampAngle);
    }

    void LateUpdate()
    {
        yaw += mouseX;
        yaw = Mathf.Repeat(yaw, 360f);
        cameraPivot.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        playerJoint.targetRotation = Quaternion.Euler(0f, -yaw, 0f);

        CacheFixedYaw();
    }


    private float fixedYaw;

    public void CacheFixedYaw()
    {
        fixedYaw = yaw;
    }

    public float GetCachedYaw()
    {
        return fixedYaw;
    }
}
