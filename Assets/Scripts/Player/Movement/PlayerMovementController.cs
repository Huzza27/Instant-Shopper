using UnityEngine;
using UnityEngine.Events;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private SyncPhysicsAnimationsLocally[] syncedObjects;

    [SerializeField] private BaseMovementBehavior currentMovement;
    [SerializeField] private StandardMovement defaultMovement;
    [SerializeField] private CartMovement cartMovement;
    [SerializeField] FPSCamera fpsCamera;



    void Start()
    {
        PlayerStateManager.Instance.OnPlayerStateChanged += UpdateMovementTypeForNewPlayerState;
        syncedObjects = transform.GetComponentsInChildren<SyncPhysicsAnimationsLocally>();
    }

    void FixedUpdate()
    {
        SyncPhysicsAnims();
    }

    void SyncPhysicsAnims()
    {
        foreach (SyncPhysicsAnimationsLocally item in syncedObjects)
        {
            item.UpdateFromJointAnimation();
        }
    }

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
}
