using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Input Reader", menuName = "Input/Input Reader")]
public class InputReader : ScriptableObject, Controls.IPlayerActions
{
    private Controls _controls;

    #region Events

    public event Action<bool> OnFireEvent;
    public event Action<Vector3> OnMoveEvent;

    public event Action<bool> OnAimEvent;

    #endregion

    private void OnEnable()
    {
        if (_controls==null)
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }
        _controls.Player.Enable();
    }

    private void OnDisable()
    {
        _controls.Player.Disable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        OnMoveEvent?.Invoke(context.ReadValue<Vector3>());
    }

    public void OnFirePrimary(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnFireEvent?.Invoke(true);
        }else if (context.canceled)
        {
            OnFireEvent?.Invoke(false);
        }
        
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAimEvent?.Invoke(true);
        }else if (context.canceled)
        {
            OnAimEvent?.Invoke(false);
        }
    }
}
