using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float targetedMoveSpeed = 10f;
    private Transform target;

    [SerializeField] private Vector2 moveInput;


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
        // input as local direction
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);

        // convert local → world using the player’s transform
        move = transform.TransformDirection(move);

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

    public void HandleMove(Vector2 input) => moveInput = input;
    public void StopMove() => moveInput = Vector2.zero;
}
