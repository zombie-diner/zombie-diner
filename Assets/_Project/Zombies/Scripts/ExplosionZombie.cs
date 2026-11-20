using System.Collections.Generic;
using UnityEngine;

public class ExplosionZombie : ZombieAI
{
    [Header("Fuse")]
    [Tooltip("Distance to the player at which the fuse is lit")]
    public float triggerRadius = 3f;

    [Tooltip("Seconds from lighting the fuse to the explosion")]
    public float fuseTime = 1.2f;

    [Tooltip("Speed multiplier once the fuse is lit (a final lunge)")]
    public float fuseSpeedMultiplier = 1.5f;

    [Header("Explosion")]
    public float blastRadius = 4f;
    public int blastDamage = 40;

    [Tooltip("Which layers the blast can hurt")]
    public LayerMask damageMask = ~0;

    [Tooltip("If checked, the blast also hurts other zombies")]
    public bool damagesZombies = false;

    [Tooltip("If checked, shooting it dead also sets it off")]
    public bool explodeOnDeath = true;

    [Tooltip("Optional particle effect or sound object spawned at the blast")]
    public GameObject explosionEffect;

    private bool fuseLit;
    private float fuseEndTime;
    private bool exploded;
    private Renderer rend;
    private Color baseColor;

    protected override void Awake()
    {
        base.Awake();
        agent.stoppingDistance = 0f;
        //look at
        rend = GetComponentInChildren<Renderer>();
        if (rend != null) baseColor = rend.material.color;
    }

    protected override void Update() 
    {
        base.Update();
        if (isDead) return;

        if (!fuseLit && target != null && FlatDistance(transform.position, target.position) <= triggerRadius)
        {
            fuseLit = true;
            fuseEndTime = Time.time + fuseTime;
            agent.speed = moveSpeed * fuseSpeedMultiplier;
        }
        if (fuseLit)
        {
            Blink();

            if (Time.time >= fuseEndTime)
            {
                Explode();
                Die();
            }
        }
    }

    protected override void Attack()
    {
    }

    // Flash between the normal color and red so players can see it's about to blow
    void Blink()
    {
        if (rend == null) return;

        float t = Mathf.PingPong(Time.time * 8f, 1f);
        rend.material.color = Color.Lerp(baseColor, Color.red, t);
    }

    protected override void Die()
    {
        if (isDead) return;

        // Shot dead before the fuse ran out: it still goes off
        if (explodeOnDeath && !exploded)
        {
            Explode();
        }
        base.Die();
    }

    void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 5f);
        }

        // Hurt each player and zombie in the blast only once, even if they have several colliders
        HashSet<Transform> hitPlayers = new HashSet<Transform>();
        HashSet<ZombieAI> hitZombies = new HashSet<ZombieAI>();
        Collider[] hits = Physics.OverlapSphere(transform.position, blastRadius, damageMask);

        foreach (Collider hit in hits)
        {
            Transform player = FindPlayerRoot(hit.transform);
            if (player != null)
            {
                if (hitPlayers.Add(player))
                {
                    DamageTarget(player, blastDamage);
                }
                continue;
            }

            if (damagesZombies)
            {
                ZombieAI zombie = hit.GetComponentInParent<ZombieAI>();
                if (zombie != null && zombie != this && hitZombies.Add(zombie))
                {
                    zombie.TakeDamage(blastDamage);
                }
            }
        }
    }

    // Colliders are often on a child, so walk up until we find the object tagged Player
    static Transform FindPlayerRoot(Transform t)
    {
        while (t != null)
        {
            if (t.CompareTag("Player")) return t;
            t = t.parent;
        }
        return null;
    }
}
