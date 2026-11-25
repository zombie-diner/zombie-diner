using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarry : MonoBehaviour
{
    //button action normally E
    [SerializeField] private InputActionReference interactAction;
    //empty child for area where food sits infront of player
    [SerializeField] private Transform holdPoint;
    //how close to interact
    [SerializeField] private float interactRange = 2f;

    public Carryable Held {get; private set;}

    private void OnEnable()
    {
        if (interactAction == null) return;
        interactAction.action.Enable();
        interactAction.action.performed += OnInteract;
    }

    private void OnDisable()
    {
        if (interactAction == null) return;
        interactAction.action.performed -= OnInteract;
        interactAction.action.Disable();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (Held != null)
        {
            PutDown();
        }
        else
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        Carryable item = FindNearestItem();
        FoodCrate crate = FindNearestCrate();

        //take closer item either item on floor/table or crate of items
        bool takeItem = item != null && (crate == null || FlatDistance(transform.position, item.transform.position) <=
         FlatDistance(transform.position, crate.transform.position));
        
        if (takeItem)
        {
            Grab(item);
        }
        else if (crate != null)
        {
            Carryable fresh = crate.Spawn(holdPoint);
            if (fresh != null) Grab(fresh);
        }
    }

    private void Grab(Carryable item)
    {
        item.PickUp(holdPoint);
        Held = item;
    }

    private void PutDown()
    {
        TableSlot slot = FindNearestFreeSlot();

        if (slot != null)
        {
            Held.PlaceOn(slot);
        }
        else
        {
            Held.DropOnFloor();
        }
        Held = null;
    }

    private Carryable FindNearestItem()
    {
        Carryable best = null;
        float bestDistance = interactRange;

        foreach (Carryable item in Carryable.All)
        {
            if (item.IsHeld) continue;

            float distance = FlatDistance(transform.position, item.transform.position);
            if (distance <= bestDistance)
            {
                bestDistance = distance;
                best = item;
            }
        }
        return best;
    }

    private FoodCrate FindNearestCrate()
    {
        FoodCrate best = null;
        float bestDistance = interactRange;
 
        foreach (FoodCrate crate in FoodCrate.All)
        {
            float distance = FlatDistance(transform.position, crate.transform.position);
            if (distance <= bestDistance)
            {
                bestDistance = distance;
                best = crate;
            }
        }
        return best;
    }

    private TableSlot FindNearestFreeSlot()
    {
        TableSlot best = null;
        float bestDistance = interactRange;

        foreach (TableSlot slot in TableSlot.All)
        {
            float distance = FlatDistance(transform.position, slot.transform.position);
            if (distance <= bestDistance)
            {
                bestDistance = distance;
                best = slot;
            }
        }
        return best;
    }

    //flat distance ignoring height
    private static float FlatDistance(Vector3 a, Vector3 b)
    {
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a,b);
    }

}
