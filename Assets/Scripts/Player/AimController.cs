using System;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class AimController : NetworkBehaviour
{
    public static AimController Instance;
    
    [Header("References")]
    [SerializeField] private InputReader _inputReader;

    [SerializeField] private CinemachineCamera ThirdPersonCamera;
    [SerializeField] private CinemachineCamera AimCamera;
    [SerializeField] private GameObject rootHead;

    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform fireTransform;


    public bool isAimingStatus;

    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private Vector3 rootHeadInitialPosition;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _inputReader.OnAimEvent += HandleAim;

        Instance = this;

        rootHeadInitialPosition = rootHead.transform.localPosition;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _inputReader.OnAimEvent -= HandleAim;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (isAimingStatus ==true)
        {
            RotateToAimCamera();
        }
    }

    private void HandleAim(bool isAiming)
    {
        SetCamera(isAiming);
    }

    private void SetCamera(bool isAiming)
    {
        isAimingStatus = isAiming;
        
        if (isAiming == true)
        {
            ThirdPersonCamera.gameObject.SetActive(false);
            AimCamera.gameObject.SetActive(true);
            rootHead.transform.localPosition = new Vector3(0.5f, 1.5f, 0f);
        }else if (isAiming == false)
        {
            ThirdPersonCamera.gameObject.SetActive(true);
            AimCamera.gameObject.SetActive(false);
            rootHead.transform.localPosition = rootHeadInitialPosition;
        }
    }

    public Vector3 AimToRayPoint()
    {
        Vector3 mouseWorldPosition = Vector3.zero;

        Vector2 ScreenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(ScreenCenterPoint);
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

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime);
    }
}
