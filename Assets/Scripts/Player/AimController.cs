using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class AimController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;

    [SerializeField] private CinemachineCamera ThirdPersonCamera;
    [SerializeField] private CinemachineCamera AimCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _inputReader.OnAimEvent += HandleAim;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _inputReader.OnAimEvent -= HandleAim;
    }

    private void HandleAim(bool isAiming)
    {
        if (isAiming == true)
        {
            ThirdPersonCamera.gameObject.SetActive(false);
            AimCamera.gameObject.SetActive(true);
        }else if (isAiming == false)
        {
            ThirdPersonCamera.gameObject.SetActive(true);
            AimCamera.gameObject.SetActive(false);
        }
    }
}
