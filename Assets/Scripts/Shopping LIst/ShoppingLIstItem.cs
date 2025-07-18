using TMPro;
using UnityEngine;

public class ShoppingLIstItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI quantityText;

    public void SetText(string itemName, int quantity)
    {
        itemNameText.text = itemName;
        quantityText.text = quantity.ToString();
    }
}
