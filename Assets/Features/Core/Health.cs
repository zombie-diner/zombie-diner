using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private GameManager gameManager;

    public bool IsDead => gameManager.IsPlayerDead;

    public void TakeDamage(float amount, GameObject source)
    {
        gameManager.DamagePlayer(amount);
    }
}
