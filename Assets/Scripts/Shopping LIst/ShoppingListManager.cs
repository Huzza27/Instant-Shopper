using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ShoppingListManager : MonoBehaviour
{
    [SerializeField] private List<ShelfItem> items;
    [SerializeField] private GameObject shoppingLisItemPrefab;
    [SerializeField] private Transform shoppingListContainer;
    [SerializeField] private UnityEvent OnListUIToggled;
    [SerializeField] private InputAction toggleShoppingListAction;


    public void Start()
    {
        toggleShoppingListAction.Enable();
        toggleShoppingListAction.performed += ctx => OnListUIToggled.Invoke();

        Dictionary<ShelfItem, int> shoppingList = GenerateShoppingList(10);
        PopulateShoppingListUI(shoppingList);
    }

    void PopulateShoppingListUI(Dictionary<ShelfItem, int> list)
    {
        foreach (ShelfItem item in list.Keys)
        {
            GameObject shoppingListItem = Instantiate(shoppingLisItemPrefab, shoppingListContainer.transform);
            shoppingListItem.transform.SetParent(shoppingListContainer);
            shoppingLisItemPrefab.GetComponent<ShoppingLIstItem>().SetText(item.GetItemData().itemName, list[item]);
        }
    }
    public Dictionary<ShelfItem, int> GenerateShoppingList(int length)
    {
        Dictionary<ShelfItem, int> shoppingList = new Dictionary<ShelfItem, int>();
        for (int i = 0; i < length; i++)
        {
            ShelfItem item = items[Random.Range(0, items.Count)];
            int quantity = Random.Range(1, 5); // Random quantity between 1 and 4
            if (!shoppingList.ContainsKey(item))
            {
                shoppingList.Add(item, quantity);

            }
            else
            {
                i--;
            }
        }
        return shoppingList;
    }
}
