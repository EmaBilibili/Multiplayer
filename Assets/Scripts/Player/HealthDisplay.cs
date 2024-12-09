using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Health _health;
    [SerializeField] private Image healthImageUI;

    public override void OnNetworkSpawn()
    {
        if (!IsClient)
        {
            return;
        }

        _health.currentHealth.OnValueChanged += HandleHealthChanged;
        
        HandleHealthChanged(0, _health.currentHealth.Value);
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient)
        {
            return;
        }
        _health.currentHealth.OnValueChanged -= HandleHealthChanged;

    }

    private void HandleHealthChanged(int oldHealth, int newHealth)
    {
        healthImageUI.fillAmount = (float)newHealth / _health.maxHealth;
    }
}
