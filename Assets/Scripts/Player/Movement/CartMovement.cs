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

    private bool cartCanRotate = true;
    private bool isResettingCamera = false;

    // cache inputs
    private float inputMouseX;
    private float inputV;

    void Start()
    {
        rightMouseDown.Enable();
        rightMouseDown.performed += _ => SetRotationFlagForCart(false);
        rightMouseDown.canceled += _ => SetRotationFlagForCart(true);
    }

    public void SetDriver(Shopper neDriver)
    {
        driver = neDriver;
    }

    void Update()
    {
        if (!currentCart) return;

        inputMouseX = Input.GetAxis("Mouse X");
        inputV = Input.GetAxis("Vertical");

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

        // 👇 New check each frame
        CheckAndAlignCartWithCamera();
    }

    void FixedUpdate()
    {
        if (!currentCart) return;
        HandleCartDriving();
    }

    private void HandleCartDriving()
    {
        float mouseX = Input.GetAxis("Mouse X");

        if (cartCanRotate)
        {
            // Cart Rotation
            Quaternion deltaRotation = Quaternion.Euler(0f, mouseX * turnSpeed * Time.deltaTime, 0f);

            Vector3 pivotToCart = currentCart.transform.position - currentCart.GetRotationPivot().position;
            Vector3 rotatedOffset = deltaRotation * pivotToCart;
            Vector3 newPosition = currentCart.GetRotationPivot().position + rotatedOffset;

            currentCart.GetCartRB().MoveRotation(deltaRotation * currentCart.GetCartRB().rotation);
            //currentCart.GetCartRB().MovePosition(newPosition);
        }
        else
        {
            // Camera Look Rotation
            currentCart.GetCameraMount().localRotation *= Quaternion.Euler(0f, mouseX * turnSpeed * Time.deltaTime, 0f);
        }

        // Forward/Backward movement
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = currentCart.transform.forward * v * moveSpeed * Time.deltaTime;
        currentCart.GetCartRB().MovePosition(currentCart.GetCartRB().position + moveDir);
    }

    void SetRotationFlagForCart(bool canRotate)
    {
        cartCanRotate = canRotate;
        if (canRotate) isResettingCamera = true;
    }

    private void CheckAndAlignCartWithCamera()
    {
        if (!currentCart) return;

        Rigidbody rb = currentCart.GetCartRB();
        bool isIdle = rb.linearVelocity.sqrMagnitude < 0.001f && Mathf.Abs(inputMouseX) < 0.01f && Mathf.Abs(inputV) < 0.01f;

        if (isIdle)
        {
            Quaternion targetRot = Quaternion.LookRotation(driver.transform.forward, Vector3.up);
            Quaternion newRot = Quaternion.Slerp(
                rb.rotation,
                targetRot,
                Time.deltaTime * cameraResetSpeed
            );

            rb.MoveRotation(newRot);
        }
    }
}
