using System.Collections;
using UnityEngine;

public class Cart : MonoBehaviour
{
    public Transform rotationPivot;
    public Transform playerStandingPoint;
    public Transform cartCameraMount;
    public Rigidbody cartRB;
    [SerializeField] LayerMask noCollideWithCart;

    [SerializeField] Transform itemDropInCartTransform;
    [SerializeField] float itemFreezeDelay = 2f;

    public void PlaceItemInShoppingCart(ShelfItem item)
    {

        GameObject newItemClone = Instantiate(item.GetItemData().prefab, itemDropInCartTransform.position, Quaternion.identity);
        newItemClone.transform.position = itemDropInCartTransform.position;
        //newItemClone.GetComponent<ShelfItem>().EnablePhysics();
        newItemClone.transform.SetParent(this.transform);

        Destroy(item.gameObject);

        //Temporray item disable

        item.gameObject.SetActive(false);
        StartCoroutine(FreezeItemInCart(newItemClone.GetComponent<ShelfItem>()));
    }

    public IEnumerator FreezeItemInCart(ShelfItem item)
    {
        yield return new WaitForSeconds(itemFreezeDelay);
        Destroy(item.gameObject.GetComponent<Rigidbody>());
        item.gameObject.layer = noCollideWithCart;
        //item.GetComponent<Collider>().enabled = false;

        //Later Inventory and other shit
    }
}
