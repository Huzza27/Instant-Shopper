using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] InputAction mouuntAction;
    [SerializeField] InputActionAsset inputActions;
    [SerializeField] InputAction moveAction;
    [SerializeField] UnityEvent onMountInputDown;
    [SerializeField] UnityEvent<Vector2> OnMove;
    [SerializeField] UnityEvent OnStopMoving;

    void OnEnable()
    {
        moveAction = inputActions.FindAction("Move");

        if (moveAction != null)
        {
            moveAction.Enable();
            moveAction.performed += OnMoveInput;
            moveAction.canceled += OnMoveInput;
        }
        else
        {
            Debug.LogWarning("Move action not found in InputActionAsset.");
        }
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        Vector2 inputVector = context.ReadValue<Vector2>();
        if (context.performed)
        {
            OnMove.Invoke(inputVector);
        }
        else if (context.canceled)
        {
            OnStopMoving.Invoke();
        }
    }

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
