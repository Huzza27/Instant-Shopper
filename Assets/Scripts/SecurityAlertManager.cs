using System.Collections.Generic;
using UnityEngine;

public class SecurityAlertManager : MonoBehaviour
{
    public static SecurityAlertManager Instance { get; private set; }

    private List<SecurityGuard> registeredGuards = new List<SecurityGuard>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void RegisterGuard(SecurityGuard guard)
    {
        if (!registeredGuards.Contains(guard))
            registeredGuards.Add(guard);
    }

    public void UnregisterGuard(SecurityGuard guard)
    {
        if (registeredGuards.Contains(guard))
            registeredGuards.Remove(guard);
    }

    public void AlertSecurity(Vector3 chaosLocation)
    {
        foreach (var guard in registeredGuards)
        {
            guard.OnChaosReported(chaosLocation);
        }
    }
}
