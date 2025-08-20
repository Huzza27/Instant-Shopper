using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] InputAction mouuntAction;
    [SerializeField] UnityEvent onMountInputDown;

    void Start()
    {
        RegisterMountInput();
    }
    
    public void RegisterMountInput()
    {
        mouuntAction.Enable();
        mouuntAction.performed += _ => onMountInputDown.Invoke();
    }
}
