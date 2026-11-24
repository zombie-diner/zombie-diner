using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxPlayerHealth = 100f;
    [SerializeField] private UIManager uiManager;

    public float PlayerHealth { get; private set; }
    public bool IsPlayerDead => PlayerHealth <= 0f;

    private void Awake()
    {
        PlayerHealth = maxPlayerHealth;
    }

    private void Start()
    {
        uiManager.UpdateHealth(PlayerHealth, maxPlayerHealth);
    }

    public void DamagePlayer(float amount)
    {
        if (IsPlayerDead) return;
        PlayerHealth = Mathf.Max(0f, PlayerHealth - Mathf.Max(0f, amount));
        uiManager.UpdateHealth(PlayerHealth, maxPlayerHealth);
    }
}
