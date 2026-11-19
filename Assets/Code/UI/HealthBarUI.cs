
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image healthFill;

    private void Update()
    {
        if (playerHealth == null || healthFill == null)
            return;

        healthFill.fillAmount = playerHealth.HealthPercentage;
    }
}
