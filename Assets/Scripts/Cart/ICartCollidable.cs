using UnityEngine;

public interface ICartCollidable
{
    public void OnCartCollision(Vector3 impactForce, GameObject objCollidedWith);
}
