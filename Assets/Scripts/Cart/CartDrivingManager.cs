// New script to attach to the cart (can be improved later with networking logic)
using UnityEngine;

public class CartDrivingManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cartCameraMount;
    [SerializeField] Transform cartGrabTransform;
    [SerializeField] private Transform cartTransform;
    [SerializeField] private Transform playerTransform;
    [SerializeField] Transform playerStandingPoint;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private FollowCamera followCamera;
    [SerializeField] Transform cartPivot;

    [Header("Driving Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 100f;

    private bool isDriving = false;
    private Transform originalCameraTarget;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleCartDriving();
        }

        if (isDriving)
        {
            HandleCartDriving();
        }
    }

    private void ToggleCartDriving()
    {
        isDriving = !isDriving;

        if (isDriving)
        {
            PlayerStateManager.Instance.SetPlayerState(PlayerState.Cart);
            GetComponent<Rigidbody>().MovePosition(cartGrabTransform.position);
            originalCameraTarget = followCamera.GetTarget();
            followCamera.SetTarget(cartCameraMount);
        }
        else
        {
            PlayerStateManager.Instance.SetPlayerState(PlayerState.Default);
            followCamera.SetTarget(originalCameraTarget);
        }
    }

    private void HandleCartDriving()
    {
        // Turning (Mouse X)
        float mouseX = Input.GetAxis("Mouse X");
        cartTransform.RotateAround(cartPivot.position, Vector3.up, mouseX * turnSpeed * Time.deltaTime);


        // Forward/Backward movement
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = cartTransform.forward * v * moveSpeed * Time.deltaTime;
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + moveDir);
        playerRB.MovePosition(playerStandingPoint.position);
    }
}
