using UnityEngine;
using UnityEngine.InputSystem;

public class CartMovement : BaseMovementBehavior
{
    Cart currentCart;
    [SerializeField] Rigidbody playerRB;
    [SerializeField] float moveSpeed;
    [SerializeField] float turnSpeed;

    [SerializeField] Shopper thisShopper;

    [SerializeField] InputAction rightMouseDown;

    [SerializeField] float cameraResetSpeed = 5f;

    private bool cartCanRotate = true;
    private bool isResettingCamera = false;

    void Start()
    {
        rightMouseDown.Enable();
        rightMouseDown.performed += ctx => SetRotationFlagForCart(false);
        rightMouseDown.canceled += ctx => SetRotationFlagForCart(true);
    }

    void Update()
    {
        HandleCartDriving();

        if (isResettingCamera)
        {
            Quaternion targetRotation = Quaternion.identity;
            currentCart.cartCameraMount.localRotation = Quaternion.Slerp(
                currentCart.cartCameraMount.localRotation,
                targetRotation,
                Time.deltaTime * cameraResetSpeed
            );

            // Stop resetting if close enough
            if (Quaternion.Angle(currentCart.cartCameraMount.localRotation, targetRotation) < 0.1f)
            {
                currentCart.cartCameraMount.localRotation = targetRotation;
                isResettingCamera = false;
            }
        }
    }

    public override void Move(Rigidbody rb)
    {
        HandleCartDriving();
    }

    public void InitializeCartInteraction(Cart cart)
    {
        currentCart = cart;
        SetCameraTarget();
    }

    void SetCameraTarget()
    {
        thisShopper.cameraFollowScript.SetTarget(currentCart.cartCameraMount);
    }

    private void HandleCartDriving()
    {
        float mouseX = Input.GetAxis("Mouse X");

        if (cartCanRotate)
        {
            // --- Cart Rotation ---
            Quaternion deltaRotation = Quaternion.Euler(0f, mouseX * turnSpeed * Time.deltaTime, 0f);

            Vector3 pivotToCart = currentCart.transform.position - currentCart.rotationPivot.position;
            Vector3 rotatedOffset = deltaRotation * pivotToCart;
            Vector3 newPosition = currentCart.rotationPivot.position + rotatedOffset;

            currentCart.cartRB.MoveRotation(deltaRotation * currentCart.cartRB.rotation);
            currentCart.cartRB.MovePosition(newPosition);
        }
        else
        {
            // --- Camera Look Rotation ---
            currentCart.cartCameraMount.localRotation *= Quaternion.Euler(0f, mouseX * turnSpeed * Time.deltaTime, 0f);
        }

        // --- Forward/Backward movement ---
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = currentCart.transform.forward * v * moveSpeed * Time.deltaTime;
        currentCart.cartRB.MovePosition(currentCart.cartRB.position + moveDir);

        // --- Player follows cart ---
        playerRB.MovePosition(currentCart.playerStandingPoint.position);
    }

    void SetRotationFlagForCart(bool canRotate)
    {
        cartCanRotate = canRotate;

        if (canRotate)
        {
            isResettingCamera = true;
        }
    }
}
