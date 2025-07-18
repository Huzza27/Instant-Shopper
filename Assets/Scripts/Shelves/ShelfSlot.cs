using UnityEngine;
public enum ShelfSize { Small, Medium, Large }

public class ShelfSlot : MonoBehaviour
{
    public ShelfSize allowedSize; // What size item can go here
    public bool isOccupied = false; // Optional for future logic
}
