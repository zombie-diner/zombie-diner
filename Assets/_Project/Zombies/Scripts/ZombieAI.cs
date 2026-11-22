using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2.5f; //new protected so subclass can see
    [SerializeField] float repathInterval = 0.25f; // how often to re-check target & path

    [Header("Attack")]
    [SerializeField] protected float attackRange = 1.5f; //new protected
    [SerializeField] float attackCooldown = 1.0f;
    [SerializeField] int attackDamage = 10;

    //new
    [Header("Health")]
    [SerializeField] protected float maxHealth = 50f;

    //new: lets a future wave manager know how many zombies are alive and when one dies
    public static int AliveCount { get; private set; }
    public static event System.Action<ZombieAI> ZombieDied;

    protected NavMeshAgent agent; //new protected
    protected Transform target; //new protected
    float repathTimer;
    float attackTimer;

    //new
    protected float health;
    protected bool isDead;

    protected virtual void Awake() //new protected virtual
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange * 0.8f; // stop just inside attack range

        //new
        health = maxHealth;
        AliveCount++;
    }

    //new
    protected virtual void OnDestroy()
    {
        AliveCount--;
    }

    protected virtual void Update() //new protected virtual
    {
        if (isDead) return; //new

        repathTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;

        // Re-pick target and update path a few times per second (not every frame)
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            target = FindTarget(); //changed bc some zombies chase food

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

    protected virtual Transform FindTarget()
    {
        return FindNearestPlayer();
    }

    protected virtual Transform FindNearestPlayer() //new protected virtual
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

    protected virtual void FaceTarget() //new protected virtual
    {
        // The agent stops rotating once it's stopped, so turn toward the player manually
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion look = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, 10f * Time.deltaTime);
    }

    protected virtual void Attack() //new protected virtual
    {
        // TODO: replace with shared IDamageable/Health once weapons person sets it up
        //Debug.Log($"{name} attacks {target.name} for {attackDamage} damage");

        DamageTarget(target, attackDamage);
    }

    //new: the ONE place zombies hurt a player. Zombie types call this too, so when the
    // weapons person finishes the shared health system, only this method needs to change.
    protected virtual void DamageTarget(Transform victim, int amount)
    {
        // TODO: replace with shared IDamageable/Health once weapons person sets it up
        Debug.Log($"{name} attacks {victim.name} for {amount} damage");
    }

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    //new
    protected virtual void Die()
    {
        if (isDead) return;

        isDead = true;
        ZombieDied?.Invoke(this);
        Destroy(gameObject);
    }

    //new: tables and counters aren't walkable, so aim for the closest walkable spot
    protected Vector3 SnapToNavMesh(Vector3 point)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, 3f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return point;
    }

    protected static float FlatDistance(Vector3 a, Vector3 b) //new protected
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