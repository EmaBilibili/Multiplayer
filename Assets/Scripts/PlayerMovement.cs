using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private CharacterController _characterController;
    private Transform _mTransform;
    private Transform mainCamera;

    [Header("Settings")] 
    [SerializeField] private float movementSpeed;
    private float rotationSmoothVelocity;
    private float rotationSmoothTime = 0.1f;


    private Vector3 previousMovementInput;

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        float x = previousMovementInput.x;
        float z = previousMovementInput.z;
        Vector3 direction = new Vector3(x, 0f, z).normalized;

        if (direction.magnitude >= .1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(_mTransform.eulerAngles.y, targetAngle, ref rotationSmoothVelocity,
                rotationSmoothTime);
            _mTransform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _characterController.Move(moveDirection * movementSpeed * Time.deltaTime);

        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _inputReader.OnMoveEvent += HandleMovement;
        _mTransform = transform;
        mainCamera = Camera.main.transform;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner)
        {
            return;
        }

        _inputReader.OnMoveEvent -= HandleMovement;
    }

    private void HandleMovement(Vector3 movementInput)
    {
        previousMovementInput = movementInput;
    }
}
