using UnityEngine;

public interface IDriver
{
    public bool IsPlayer();
    public GameObject GetObject();
    public void SetDrivable(IDriveable driver);   
}
