using System.Collections.Generic;
using UnityEngine;

public class CashRegisterManager : MonoBehaviour
{
    public static CashRegisterManager Instance;
    [SerializeField] private List<GameObject> allRegisters;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public ICashRegister GetRandomRegister()
    {
        return allRegisters[Random.Range(0, allRegisters.Count)].GetComponent<ICashRegister>();
    }
}
