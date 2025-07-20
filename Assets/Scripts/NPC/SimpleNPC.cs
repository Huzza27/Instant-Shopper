using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class SimpleNPC : MonoBehaviour, INPCBehavior
{
    public float wanderRadius = 10f;
    public float itemCollectionDelay;
    public float moveToNewShelfDelay;
    private NavMeshAgent agent;
    private int numItemsToCollect = 10;
    private int itemsCollected = 0;
    [SerializeField] private Cart cart;

    private List<ShelfItem> shopperInventory = new List<ShelfItem>();

    void Start()
    {
        Initialize();
    }

    private IEnumerator ShopRoutine()
    {
        while (itemsCollected < numItemsToCollect)
        {
            Shelf shelf = GetShelf();
            ShelfSlot slot = GetNewSlot(shelf);
            MoveToNewPosition(slot.GetAIShelfTransform());

            // Wait until the agent arrives
            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                yield return null;
            }

            // Now collect items
            CollectItemsOffShelf(slot);
            // Wait a short time before going to next shelf (optional)
            yield return new WaitForSeconds(moveToNewShelfDelay);
        }

        Debug.Log("Shopping complete!");
    }

    void PlaceItemInCart(ShelfItem item)
    {
        cart.PlaceItemInShoppingCart(item);
    }

    public void MoveToNewPosition(Transform targetPosition)
    {
        agent.SetDestination(targetPosition.position);
    }

    public void CollectItemsOffShelf(ShelfSlot slot)
    {
        List<ShelfItem> itemsInSlot = slot.GetItemsInSlot();
        int maxItemsCanTake = Mathf.Min(itemsInSlot.Count, numItemsToCollect - itemsCollected);

        if (maxItemsCanTake <= 0)
            return;

        int itemsToCollect = Random.Range(1, maxItemsCanTake + 1); // Always at least 1

        for (int i = 0; i < itemsToCollect; i++)
        {
            ShelfItem item = itemsInSlot[0];
            StartCoroutine(CollectItem(slot, item));
            Debug.Log($"Collected item: {item.name}. Total collected: {itemsCollected}");

            if (itemsCollected >= numItemsToCollect)
                break;
        }
    }

    IEnumerator CollectItem(ShelfSlot slot, ShelfItem item)
    {
        yield return new WaitForSeconds(itemCollectionDelay);
            slot.RemoveItemFromSlot();
            shopperInventory.Add(item);
            PlaceItemInCart(item);
            itemsCollected++;
    }


    public ShelfSlot GetNewSlot(Shelf shelf)
    {
        ShelfSlot randomSlot = shelf.GetRandomSlot();
        Debug.Assert(randomSlot != null, "No slots available in the shelf.");
        return randomSlot;
    }


    Shelf GetShelf()
    {
        Shelf shelf = ShelfManager.Instance.GetRandomShelf();
        Debug.Assert(shelf != null, "No shelves available in ShelfManager.");
        return shelf;

    }

    public void Initialize()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(ShopRoutine());
    }


    public void ReactToChaos()
    {
        throw new System.NotImplementedException();
    }
}
