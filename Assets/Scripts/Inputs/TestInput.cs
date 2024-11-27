using System;
using UnityEngine;

public class TestInput : MonoBehaviour
{
    [SerializeField] private InputReader _inputReader;

    private void OnEnable()
    {
        _inputReader.OnMoveEvent += HandleMovement;
    }

    private void HandleMovement(Vector3 movement)
    {
        Debug.Log("Movement: "+ movement);
    }
}
