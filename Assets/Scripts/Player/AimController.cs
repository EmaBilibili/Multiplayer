using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class AimController : NetworkBehaviour
{
    public static AimController instance;
    
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private CinemachineCamera ThirdPersonCamera;
    [SerializeField] private CinemachineCamera AimCamera;
    [SerializeField] private Transform fireTransform;

    [SerializeField] private float rotationSpeed = 20f;
    public bool isAimingStatus;
    


    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (isAimingStatus == true)
        {
            RotateToAimCamera();
        }
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _inputReader.OnAimEvent += HandleAim;

        instance = this;
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
        SetCamera(isAiming);
    }

    private void SetCamera(bool isAiming)
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

    public Vector3 AimToRayPoint()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            fireTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        return mouseWorldPosition;
    }

    private void RotateToAimCamera()
    {
        Vector3 aimTarget = AimToRayPoint();

        aimTarget.y = transform.position.y;
        Vector3 aimDirection = (aimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotationSpeed);
    }
}
