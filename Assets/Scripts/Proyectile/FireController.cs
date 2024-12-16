using System;
using Unity.Netcode;
using UnityEngine;

public class FireController : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject projectileClient;
    [SerializeField] private GameObject projectileServer;

    [Header("Settings")] 
    [SerializeField] private float projectileSpeed = 10f;

    private bool isFiring;

    public override void OnNetworkSpawn()
    {
        _inputReader.OnFireEvent += HandleFirePrimary;
    }

    public override void OnNetworkDespawn()
    {
        _inputReader.OnFireEvent -= HandleFirePrimary;
    }

    private void Update()
    {
        if (!IsOwner || !isFiring || !AimController.Instance.isAimingStatus)
        {
            return;
        }

        Fire();
    }

    private void HandleFirePrimary(bool isFiring)
    {
        this.isFiring = isFiring;
        Debug.Log(isFiring);
    }

    private void Fire()
    {
        Vector3 mouseWorldPosition = AimController.Instance.AimToRayPoint();
        Vector3 aimDirection = (mouseWorldPosition - projectileSpawnPoint.position).normalized;

        SpawnProjectileLocal(projectileSpawnPoint.position, aimDirection, true);
        SpawnProjectileServerRPC(projectileSpawnPoint.position, aimDirection);
        isFiring = false; // Detener el disparo tras una ejecución
    }

    private void SpawnProjectileLocal(Vector3 position, Vector3 direction, bool isClient)
    {
        GameObject projectilePrefab = isClient ? projectileClient : projectileServer;
        Instantiate(projectilePrefab, position, Quaternion.LookRotation(direction, Vector3.up));
    }

    [ServerRpc]
    private void SpawnProjectileServerRPC(Vector3 position, Vector3 direction)
    {
        // Instanciar el proyectil del servidor
        GameObject projectileInstance = Instantiate(projectileServer, position, Quaternion.LookRotation(direction, Vector3.up));

        // Configurar el OwnerClientId en el componente DealDamage
        if (projectileInstance.TryGetComponent<DealDamage>(out DealDamage damage))
        {
            damage.SetOwner(OwnerClientId);
        }

        // Llamar al ClientRPC para sincronizar el proyectil en los clientes
        SpawnProjectileClientRPC(position, direction);
    }


    [ClientRpc]
    private void SpawnProjectileClientRPC(Vector3 position, Vector3 direction)
    {
        if (!IsOwner)
        {
            SpawnProjectileLocal(position, direction, true);
        }
    }
}
