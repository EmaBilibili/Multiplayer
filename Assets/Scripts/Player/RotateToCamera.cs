using System;
using Unity.Netcode;
using UnityEngine;

public class RotateToCamera : NetworkBehaviour
{
    private Camera _camera;

    public override void OnNetworkSpawn()
    {
            _camera = Camera.main;
    }
    
    private void Update()
    {
        FaceCamera();
    }

    private void FaceCamera()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
    }
}
