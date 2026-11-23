using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Player HUD")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI cashText;

    [Header("Wave Info")]
    [SerializeField] private TextMeshProUGUI waveText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthSlider.value = currentHealth / maxHealth;
    }

    public void UpdateAmmo(int currentAmmo, int reserveAmmo, bool isInfinite = false)
    {
        ammoText.text = isInfinite ? "∞" : $"{currentAmmo} / {reserveAmmo}";
    }

    public void UpdateCash(int amount)
    {
        cashText.text = $"${amount}";
    }

    public void UpdateWave(string statusText)
    {
        waveText.text = statusText;
    }
}