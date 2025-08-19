using UnityEngine;

public interface IThrowable
{
    void ThrowItem(float throwForce);
    public Vector3 GetThrowVelocity();
}
