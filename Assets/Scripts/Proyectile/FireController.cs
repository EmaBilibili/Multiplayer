using System;
using Unity.Netcode;
using UnityEngine;

public class FireController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader; // Para manejar los eventos de entrada del jugador.
    [SerializeField] private Transform projectileSpawnPoint; // Punto de aparición del proyectil.
    [SerializeField] private GameObject projectilePrefab; // Prefab del proyectil sincronizado en red.

    [Header("Settings")]
    [SerializeField] private float projectileSpeed = 10f; // Velocidad del proyectil.

    private bool isFiring;
    private Vector3 mouseWorldPosition;

    public override void OnNetworkSpawn()
    {
        // Suscripción al evento de disparo.
        if (IsOwner)
        {
            _inputReader.OnFireEvent += HandleFirePrimary;
        }
    }

    public override void OnNetworkDespawn()
    {
        // Desuscripción al evento de disparo.
        if (IsOwner)
        {
            _inputReader.OnFireEvent -= HandleFirePrimary;
        }
    }

    private void Update()
    {
        // Solo el propietario del objeto ejecuta la lógica de disparo.
        if (!IsOwner || !isFiring) return;

        Fire();
        isFiring = false; // Restablece el estado de disparo después de ejecutar.
    }

    private void HandleFirePrimary(bool isFiring)
    {
        // Actualiza el estado de disparo según el input.
        this.isFiring = isFiring;
        Debug.Log($"Firing state: {isFiring}");
    }

    private void Fire()
    {
        // Calcula la posición y dirección del disparo basándose en el apuntado.
        mouseWorldPosition = AimController.instance.AimToRayPoint();
        Vector3 aimDirection = (mouseWorldPosition - projectileSpawnPoint.position).normalized;

        // Solicita al servidor la creación del proyectil.
        FireProjectileServerRpc(projectileSpawnPoint.position, aimDirection);
    }

    [ServerRpc]
    private void FireProjectileServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        // Instancia el proyectil en el servidor y sincroniza con los clientes.
        GameObject projectileInstance = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
        Rigidbody projectileRb = projectileInstance.GetComponent<Rigidbody>();
        
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = direction * projectileSpeed;
        }

        NetworkObject networkObject = projectileInstance.GetComponent<NetworkObject>();
        networkObject.Spawn(); // Sincroniza el proyectil con todos los clientes.
    }

    
}
