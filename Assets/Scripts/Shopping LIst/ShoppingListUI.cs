using UnityEngine;
using UnityEngine.InputSystem;

public class ShoppingListUI : MonoBehaviour
{
    [SerializeField] private GameObject shoppingListPanel;

    public void ToggleShoppingList()
    {
        if (shoppingListPanel.activeSelf)
        {
            shoppingListPanel.SetActive(false);
        }
        else
        {
            shoppingListPanel.SetActive(true);
        }
    }
}
