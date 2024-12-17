using System;
using Unity.Netcode;
using UnityEngine;

public class Charger : NetworkBehaviour
{
    public NetworkVariable<int> TotalAmmo = new NetworkVariable<int>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Ammo>(out Ammo ammo))
        {
            return;
        }

        int ammoValue = ammo.Collect();

        if (!IsServer)
        {
            return;
        }

        TotalAmmo.Value += ammoValue;
    }
}
