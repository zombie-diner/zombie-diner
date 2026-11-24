using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 2.5f;
    [SerializeField] float repathInterval = 0.25f; // how often to re-check target & path

    [Header("Attack")]
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float attackCooldown = 1.0f;
    [SerializeField] int attackDamage = 10;

    NavMeshAgent agent;
    Transform target;
    float repathTimer;
    float attackTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange * 0.8f; // stop just inside attack range
    }

    void Update()
    {
        repathTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        // Re-pick target and update path a few times per second (not every frame)
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            target = FindNearestPlayer();

            if (target != null)
                agent.SetDestination(target.position);
            else if (agent.hasPath)
                agent.ResetPath();
        }

        if (target == null) return;

        if (FlatDistance(transform.position, target.position) <= attackRange)
        {
            FaceTarget();
            if (attackTimer <= 0f)
            {
                Attack();
                attackTimer = attackCooldown;
            }
        }
    }

    Transform FindNearestPlayer()
    {
        // TEMP: uses the "Player" tag. Swap to the team's player registry once it exists.
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Transform nearest = null;
        float best = float.MaxValue;

        foreach (GameObject p in players)
        {
            float d = FlatDistance(transform.position, p.transform.position);
            if (d < best)
            {
                best = d;
                nearest = p.transform;
            }
        }
        return nearest;
    }

    void FaceTarget()
    {
        // The agent stops rotating once it's stopped, so turn toward the player manually
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
    }

    void Attack()
    {
        // TODO: replace with shared IDamageable/Health once weapons person sets it up
        Debug.Log($"{name} attacks {target.name} for {attackDamage} damage");
    }

    static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f; b.y = 0f;
        return Vector3.Distance(a, b);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}