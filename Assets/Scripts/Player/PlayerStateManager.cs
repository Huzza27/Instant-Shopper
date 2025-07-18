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

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager Instance;
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

    public PlayerState GetPlayerState()
    {
        return playerState;
    }

    public static Shopper GetShopper()
    {
        return shopper;
    }
}
