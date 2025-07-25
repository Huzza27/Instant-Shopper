using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ShoppingListManager : MonoBehaviour
{
    [SerializeField] private List<ShelfItem> items;
    [SerializeField] private UnityEvent OnListUIToggled;
    [SerializeField] private UnityEvent<ShelfItemData, int> OnItemRemoveFromList;
    [SerializeField] private UnityEvent<ShelfItemData, int> OnItemAddedToList;
    [SerializeField] private InputAction toggleShoppingListAction;

    private Dictionary<ShelfItemData, int> shoppingList;

    private void Start()
    {
        toggleShoppingListAction.Enable();
        toggleShoppingListAction.performed += ctx => OnListUIToggled.Invoke();

        GenerateShoppingList(10);
        //PopulateShoppingListUI();
    }

    private void GenerateShoppingList(int length)
    {
        shoppingList = new Dictionary<ShelfItemData, int>();
        for (int i = 0; i < length; i++)
        {
            ShelfItem item = items[Random.Range(0, items.Count)];
            int quantity = Random.Range(1, 5); // Random quantity between 1 and 4

            if (!shoppingList.ContainsKey(item.GetItemData()))
            {
                shoppingList.Add(item.GetItemData(), quantity);
            }
            else
            {
                i--; // Retry to ensure unique items
            }
        }
    }

    private void PopulateShoppingListUI()
    {
        foreach(ShelfItemData data in shoppingList.Keys)
        {
            OnItemAddedToList.Invoke(data, shoppingList[data]);
        }
    }

    public void RemoveItemFromList(ShelfItemData item)
    {
        if (shoppingList.ContainsKey(item))
        {
            shoppingList[item]--;

            OnItemRemoveFromList.Invoke(item, shoppingList[item]);

            if (shoppingList[item] <= 0)
            {
                shoppingList.Remove(item);
            }
        }
    }
}
