using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerHealthText;
    [SerializeField] TextMeshProUGUI ammoText;

    void OnEnable()
    {
        PlayerHealthController.OnHealthChanged += PlayerHealthController_OnHealthChanged;
        Gun.OnAmmoChanged += Gun_OnAmmoChanged;
    }

    void OnDisable()
    {
        PlayerHealthController.OnHealthChanged -= PlayerHealthController_OnHealthChanged;
        Gun.OnAmmoChanged -= Gun_OnAmmoChanged;
    }

    void PlayerHealthController_OnHealthChanged(float health)
    {
        playerHealthText.text = $"HP: {health}";
    }

    void Gun_OnAmmoChanged(int ammo, int maxAmmo)
    {
        ammoText.text = $"Ammo: {ammo}/{maxAmmo}";
    }
}
