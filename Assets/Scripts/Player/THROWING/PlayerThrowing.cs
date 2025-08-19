using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerThrowing : MonoBehaviour
{
    [SerializeField] InputAction throwInput;

    void OnEnable()
    {
        throwInput.Enable();
        throwInput.performed += ThrowObject;
    }

    public void ThrowObject(InputAction.CallbackContext context)
    {
        Shopper shopper = GetComponent<Shopper>();
        shopper.ThrowObject();
    }
}
