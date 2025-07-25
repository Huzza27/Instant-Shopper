using TMPro;
using UnityEngine;

public class ShoppingLIstItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private ShelfItemData itemData;

    public void Initialize(ShelfItemData item, int quantity)
    {
        SetItem(item);
        UpdateText(item, quantity);
    }
    public void SetItem(ShelfItemData item)
    {
        itemData = item;
    }
    public void UpdateText(ShelfItemData item, int quantity)
    {
        itemNameText.text = item.itemName;
        quantityText.text = quantity.ToString();
    }
}
