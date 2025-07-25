using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EmployeeCashRegister : MonoBehaviour, ICashRegister
{
    [SerializeField] private UnityEvent<ShelfItemData> OnItemPurschased; 
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ICart>(out ICart cart))
        {
            RegisterItemPurchase(cart);
        }
    }
    public void RegisterItemPurchase(ICart cart)
    {
        foreach (ShelfItem item in cart.GetItems())
        {
            OnItemPurschased.Invoke(item.GetItemData());
        }
        cart.ClearCartInventory();
    }
}
