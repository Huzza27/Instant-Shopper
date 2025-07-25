using UnityEngine;
using UnityEngine.InputSystem;

public class CartMovement : BaseMovementBehavior
{
    [SerializeField] PlayerCart currentCart; //Temporary reference to the cart being driven
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
            currentCart.GetCameraMount().localRotation = Quaternion.Slerp(
                currentCart.GetCameraMount().localRotation,
                targetRotation,
                Time.deltaTime * cameraResetSpeed
            );

            // Stop resetting if close enough
            if (Quaternion.Angle(currentCart.GetCameraMount().localRotation, targetRotation) < 0.1f)
            {
                currentCart.GetCameraMount().localRotation = targetRotation;
                isResettingCamera = false;
            }
        }
    }

    public override void Move(Rigidbody rb)
    {
        HandleCartDriving();
    }

    public void InitializeCartInteraction(ICart cart)
    {
        if(cart is PlayerCart playerCart)
        {
            currentCart = playerCart;
        }
        else
        {
            Debug.LogError("CartMovement can only handle PlayerCart instances.");
            return;
        }
        SetCameraTarget();
    }

    void SetCameraTarget()
    {
        thisShopper.cameraFollowScript.SetTarget(currentCart.GetCameraMount());
    }

    private void HandleCartDriving()
    {
        float mouseX = Input.GetAxis("Mouse X");

        if (cartCanRotate)
        {
            // --- Cart Rotation ---
            Quaternion deltaRotation = Quaternion.Euler(0f, mouseX * turnSpeed * Time.deltaTime, 0f);

            Vector3 pivotToCart = currentCart.transform.position - currentCart.GetRotationPivot().position;
            Vector3 rotatedOffset = deltaRotation * pivotToCart;
            Vector3 newPosition = currentCart.GetRotationPivot().position + rotatedOffset;

            currentCart.GetCartRB().MoveRotation(deltaRotation * currentCart.GetCartRB().rotation);
            currentCart.GetCartRB().MovePosition(newPosition);
        }
        else
        {
            // --- Camera Look Rotation ---
            currentCart.GetCameraMount().localRotation *= Quaternion.Euler(0f, mouseX * turnSpeed * Time.deltaTime, 0f);
        }

        // --- Forward/Backward movement ---
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = currentCart.transform.forward * v * moveSpeed * Time.deltaTime;
        currentCart.GetCartRB().MovePosition(currentCart.GetCartRB().position + moveDir);

        // --- Player follows cart ---
        playerRB.MovePosition(currentCart.GetStandingPoint().position);
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
