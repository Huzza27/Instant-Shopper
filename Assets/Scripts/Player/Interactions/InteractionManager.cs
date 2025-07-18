using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractionManager : MonoBehaviour
{
    [Header("CONTEXT")]
    [SerializeField] private GameObject tempPlayerReference;
    [SerializeField] private Transform cartTransform;
    [SerializeField] private Transform itemHoldingTransform;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask layer;
    private InputAction interactAction;
    private InteractionContexzt interactableContext;
    public Camera camera; //Temporary for testing



    void Start()
    {
        RegisterEButtonInput();
        interactableContext = CreateInteractableContext();
    }

    InteractionContexzt CreateInteractableContext()
    {
        InteractionContexzt context = new InteractionContexzt
        {
            player = tempPlayerReference,
            itemHoldPoint = itemHoldingTransform,
            cartHandlePoint = cartTransform
        };
        return context;
    }

    void RegisterEButtonInput()
    {
        interactAction = new InputAction(type: InputActionType.Button);
        interactAction.AddBinding("<Keyboard>/e");
        interactAction.Enable();
        interactAction.performed += InteractWithItem;

    }

    public void InteractWithItem(InputAction.CallbackContext context)
    {
        Debug.Log("E PRESSED");
        GetInteractableObject().Interact(interactableContext, tempPlayerReference.GetComponent<Shopper>());
    }

    public IInteractable GetInteractableObject()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, maxDistance, layer))
        {
            return hit.collider.GetComponent<IInteractable>();
        }
        return null;
    }
}
