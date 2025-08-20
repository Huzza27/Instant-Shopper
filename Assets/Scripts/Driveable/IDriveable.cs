using System;
using UnityEngine;

public interface IDriveable
{
    Transform GetSeatTransform();
    public void Mount(Shopper shopper);
    public void Dismount(Shopper shopper, System.Action onDismountComplete = null);
    public void EnableMotor();
}
