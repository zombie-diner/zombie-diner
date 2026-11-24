using UnityEngine;

public class FoodZombie : ZombieAI
{
    [Header("Eating")]
    [Tooltip("Bites needed to finish a dish. Eating time is about (bites - 1) x Attack Cooldown.")]
    public int bitesToFinish = 4;

    [Tooltip("If checked, with no food left it chases and attacks players like a basic zombie")]
    public bool attackPlayersWhenNoFood = true;

    private Edible food;
    private int bites;

    protected override Transform FindTarget() 
    {
        // Keep the dish already chosen, unless a player picked it up or it was destroyed
        if (food != null && food.CanBeClaimedBy(this))
        {
            return food.transform;
        }
        ReleaseFood();

        food = FindNearestFood();
        if (food != null)
        {
            food.Claim(this);
            agent.stoppingDistance = 0.2f;   // walk all the way to the table edge
            return food.transform;
        }

        agent.stoppingDistance = attackRange * 0.8f;
        return attackPlayersWhenNoFood ? FindNearestPlayer() : null;
    }

    // Aimed at a dish this is a bite; aimed at a player it is the normal attack
    protected override void Attack()
    {
        if (food != null && target == food.transform)
        {
            Bite();
        }
        else
        {
            base.Attack();
        }
    }

    void Bite()
    {
        bites++;
        if (bites >= bitesToFinish)
        {
            Edible eaten = food;
            ReleaseFood();
            target = null;
            eaten.Consume();
        }
    }

    Edible FindNearestFood()
    {
        Edible best = null;
        float bestDistance = float.MaxValue;

        foreach (Edible candidate in Edible.All)
        {
            if (!candidate.CanBeClaimedBy(this)) continue;

            float distance = FlatDistance(transform.position, candidate.transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
            }
        }
        return best;
    }

    void ReleaseFood()
    {
        if (food != null) food.Release(this);
        food = null;
        bites = 0;
    }

    protected override void Die()
    {
        ReleaseFood();
        base.Die();
    }
}
