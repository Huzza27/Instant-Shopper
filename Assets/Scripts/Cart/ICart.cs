using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface ICart
{
    Transform GetDropTransform();
    Transform GetCartTransform();
    Transform GetStandingPoint();
    NavMeshAgent GetNavMeshAgent();
    List<ShelfItem> GetItems();
    void PlaceItemInShoppingCart(ShelfItem item);
    void OnItemPlaced(GameObject item);
    public void ClearCartInventory();
    public void NPCMount(IShopperNPC npc);
}
