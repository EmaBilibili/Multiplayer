using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class FireController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader; // Para manejar los eventos de entrada del jugador.
    [SerializeField] private Transform projectileSpawnPoint; // Punto de aparición del proyectil.
    [SerializeField] private GameObject projectileClientPrefab; // Proyectil visual para el cliente.
    [SerializeField] private GameObject projectileServerPrefab; // Proyectil lógico para el servidor.

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
        // Solo el propietario ejecuta la lógica de disparo.
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
        // Calcula la posición y dirección del disparo.
        mouseWorldPosition = AimController.instance.AimToRayPoint();
        Vector3 aimDirection = (mouseWorldPosition - projectileSpawnPoint.position).normalized;

        // Crear proyectil local para el cliente.
        SpawnDummyProjectile(projectileSpawnPoint.position, aimDirection);

        // Solicitar al servidor la creación del proyectil lógico.
        SpawnProjectileServerRpc(projectileSpawnPoint.position, aimDirection);
    }

    private void SpawnDummyProjectile(Vector3 spawnPosition, Vector3 direction)
    {
        Debug.Log($"Spawning dummy projectile at {spawnPosition} with direction {direction}");

        GameObject dummyProjectile = Instantiate(
            projectileClientPrefab,
            spawnPosition,
            direction.sqrMagnitude > 0 ? Quaternion.LookRotation(direction) : Quaternion.identity
        );

        Rigidbody rb = dummyProjectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        // Destruir el proyectil visual después de un tiempo (opcional).
        Destroy(dummyProjectile, 2f);
    }

    [ServerRpc]
    private void SpawnProjectileServerRpc(Vector3 spawnPosition, Vector3 direction)
    {
        // Crear el proyectil lógico en el servidor.
        GameObject serverProjectile = Instantiate(
            projectileServerPrefab,
            spawnPosition,
            direction.sqrMagnitude > 0 ? Quaternion.LookRotation(direction) : Quaternion.identity
        );

        Rigidbody rb = serverProjectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        // Sincronizar el proyectil oficial con los clientes.
        NetworkObject networkObject = serverProjectile.GetComponent<NetworkObject>();
        networkObject.Spawn();

        // Notificar a los clientes para crear un proyectil visual.
        NotifyClientsOfProjectileClientRpc(networkObject.NetworkObjectId);

        if (serverProjectile.TryGetComponent<DealDamage>(out DealDamage damage))
        {
            damage.SetOwner(OwnerClientId);
        }
    }

    [ClientRpc]
    private void NotifyClientsOfProjectileClientRpc(ulong networkObjectId)
    {
        // Crear el proyectil visual para clientes no propietarios.
        if (IsOwner) return;

        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
        if (networkObject != null)
        {
            SpawnVisualProjectile(networkObject.transform);
        }
    }

    private void SpawnVisualProjectile(Transform serverTransform)
    {
        // Instanciar el proyectil visual y sincronizarlo con el oficial.
        GameObject visualProjectile = Instantiate(
            projectileClientPrefab,
            serverTransform.position,
            serverTransform.rotation
        );

        // Sincronizar continuamente la posición con el servidor.
        StartCoroutine(SyncWithServerProjectile(visualProjectile.transform, serverTransform));
    }

    private IEnumerator SyncWithServerProjectile(Transform visualTransform, Transform serverTransform)
    {
        while (serverTransform != null)
        {
            visualTransform.position = serverTransform.position;
            visualTransform.rotation = serverTransform.rotation;
            yield return null; // Esperar al siguiente frame.
        }
    }
}
