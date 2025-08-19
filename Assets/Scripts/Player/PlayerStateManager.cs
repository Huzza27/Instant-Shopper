using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public enum PlayerState
{
    Default,
    Cart,
    Ragdoll
}

public enum MovementMode
{
    Default,
    Targeted
}

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager Instance;
    [SerializeField] private MovementMode movementMode = MovementMode.Default;
    [SerializeField] private PlayerState playerState = PlayerState.Default;
    [SerializeField] static Shopper shopper;
    public Action OnPlayerStateChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SetPlayerState(PlayerState state)
    {
        playerState = state;
        OnPlayerStateChanged?.Invoke();
    }

    public void SetMovementMode(MovementMode mode)
    {
        movementMode = mode;
    }

    public PlayerState GetPlayerState()
    {
        return playerState;
    }

    public MovementMode GetMovementMode()
    {
        return movementMode;
    }

    public static Shopper GetShopper()
    {
        return shopper;
    }
}
