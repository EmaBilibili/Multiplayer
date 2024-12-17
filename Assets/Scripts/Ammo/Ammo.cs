using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public abstract class Ammo : NetworkBehaviour
{
    protected int ammoValue = 10;
    protected bool isCollected;

    public abstract int Collect();

    public void SetValue(int value)
    {
        ammoValue = value;
    }

    protected void Show(bool status)
    {
        gameObject.SetActive(status);
    }
}
