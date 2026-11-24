using UnityEngine;
using UnityEngine.InputSystem;

// TEMP testing tool: K = damage the zombie nearest the player, L = kill every zombie
public class DebugZombieKiller : MonoBehaviour
{
    [SerializeField] float damage = 999f;

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.kKey.wasPressedThisFrame) DamageNearest();
        if (Keyboard.current.lKey.wasPressedThisFrame) KillAll();
    }

    void DamageNearest()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;

        ZombieAI nearest = null;
        float best = float.MaxValue;

        foreach (ZombieAI z in FindObjectsByType<ZombieAI>())
        {
            float d = Vector3.Distance(player.transform.position, z.transform.position);
            if (d < best) { best = d; nearest = z; }
        }

        if (nearest != null)
        {
            Debug.Log($"[Debug] Hit {nearest.name} for {damage}");
            nearest.TakeDamage(damage);
        }
    }

    void KillAll()
    {
        foreach (ZombieAI z in FindObjectsByType<ZombieAI>())
            z.TakeDamage(999999f);
    }
}