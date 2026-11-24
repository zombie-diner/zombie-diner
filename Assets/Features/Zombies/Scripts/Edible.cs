using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// Put this on any food item or dish that zombies should be able to eat.
// The cooking code only needs to do two things with it:
//   1. Call SetOnTable(false) when a player picks the food up, and SetOnTable(true) when it is put down on a table.
//   2. Hook up the onEaten event (in the Inspector) to free the table slot or update the orders.
public class Edible : MonoBehaviour
{
    // Every enabled Edible in the scene, so zombies can find the nearest one
    public static readonly List<Edible> All = new List<Edible>();

    [Tooltip("Activates when a zombie finishes eating this food, right before it is destroyed")]
    public UnityEvent onEaten;

    // True while the food is sitting on a table. Zombies ignore food being carried.
    public bool IsOnTable { get; private set; } = true;

    // The zombie currently heading to this food, so two zombies don't chase the same dish
    public ZombieAI ClaimedBy { get; private set; }

    void OnEnable()
    {
        All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    public void SetOnTable(bool onTable)
    {
        IsOnTable = onTable;
    }

    public bool CanBeClaimedBy(ZombieAI zombie)
    {
        return IsOnTable && (ClaimedBy == null || ClaimedBy == zombie);
    }

    public void Claim(ZombieAI zombie)
    {
        ClaimedBy = zombie;
    }

    public void Release(ZombieAI zombie)
    {
        if (ClaimedBy == zombie) ClaimedBy = null;
    }

    // Called when a zombie has finished eating. Swap this for the cooking system's own
    // "remove dish" logic if it needs something other than Destroy.
    //probably the final though unless we do something specific
    public void Consume()
    {
        onEaten?.Invoke();
        Destroy(gameObject);
    }
}
