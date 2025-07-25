using UnityEngine;
public class CartItemPlacer : MonoBehaviour
{
    [SerializeField] private float freezeDelay = 2f;
    [SerializeField] private LayerMask noCollideWithCart;


    private IItemSpawner itemSpawner;

    private void Start()
    {
        itemSpawner = ItemSpawnerService.Instance;
    }

    public void PlaceInCart(ShelfItem shelfItem, ICart cart)
    {
        ShelfItemData itemData = shelfItem.GetItemData();

        GameObject itemClone = itemSpawner.SpawnItem(
            itemData,
            cart.GetDropTransform()
        );

        Destroy(shelfItem.gameObject);
        cart.OnItemPlaced(itemClone);
    }
}
