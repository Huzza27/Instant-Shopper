using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class ShoppingListUIManager : MonoBehaviour
{
    [SerializeField] private GameObject shoppingLisItemPrefab;
    [SerializeField] private Transform shoppingListContainer;
    private Dictionary<ShelfItemData, ShoppingLIstItem> shoppingListUIEntries;

    private void Awake()
    {
        shoppingListUIEntries = new Dictionary<ShelfItemData, ShoppingLIstItem>();
    }
    public void UpdateListUI(ShelfItemData data, int quantity)
    {
        GameObject shoppingListItemGO = Instantiate(shoppingLisItemPrefab, shoppingListContainer.position, Quaternion.identity, shoppingListContainer.transform);
        ShoppingLIstItem listItem = shoppingListItemGO.GetComponent<ShoppingLIstItem>();
        listItem.Initialize(data, quantity);
        shoppingListUIEntries[data] = listItem;
    }

    public void RemoveItemFromList(ShelfItemData item, int quantity)
    {
        ShoppingLIstItem listItem = shoppingListUIEntries[item];
        listItem.UpdateText(item, quantity);

    }
}
