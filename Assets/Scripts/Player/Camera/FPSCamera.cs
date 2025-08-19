// FPSCamera: attach to a child GameObject holding the Camera.
// Put PlayerMotor on the parent. Set playerBody = parent transform.
using UnityEngine;

public class FPSCamera : MonoBehaviour
{
    public float sensitivity = 200f;
    public Transform playerBody;

    float xRot;

    void Start() => Cursor.lockState = CursorLockMode.Locked;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -89f, 89f);
        transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
