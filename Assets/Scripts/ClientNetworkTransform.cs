using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        // Retornamos falso para indicar que la autoridad está del lado del cliente.
        return false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // Solo permitimos que el propietario controle el transform.
        CanCommitToTransform = IsOwner;
    }
}