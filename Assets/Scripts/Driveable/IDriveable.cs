using System;
using UnityEngine;

public interface IDriveable
{
    Transform GetSeatTransform();
    public void Mount(IDriver shopper, System.Action onMountComplete = null);
    public void Dismount(Shopper shopper, System.Action onDismountComplete = null);
    public void EnableMotor();
}
