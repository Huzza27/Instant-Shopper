using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float targetedMoveSpeed = 10f;
    private Transform target;

    //PlayerStateManager.Instance.OnPlayerStateChanged += UpdateMovementTypeForNewPlayerState;

    /*
    void UpdateMovementTypeForNewPlayerState()
    {
        defaultMovement.enabled = false;
        cartMovement.enabled = false;

        switch (PlayerStateManager.Instance.GetPlayerState())
        {
            case PlayerState.Default:
                defaultMovement.enabled = true;
                break;
            case PlayerState.Cart:
                cartMovement.enabled = true;
                break;
        }
    }
    */
    void Update()
    {
        if (PlayerStateManager.Instance.GetMovementMode() == MovementMode.Targeted)
        {
            TargetedMovement();
        }
        else
        {
            RegularMovement();
        }

    }
    public void RegularMovement()
    {
        // Get input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // Move relative to player's facing direction
        Vector3 move = transform.right * h + transform.forward * v;

        // Apply movement
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    public void TargetedMovement()
    {
        if (target == null) return;
        controller.enabled = false;
        transform.position = target.position;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
