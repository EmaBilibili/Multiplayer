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


}
