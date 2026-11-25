using System.Collections.Generic;
using UnityEngine;

//put on anything a player can pickup with a Rigidbody

[RequireComponent(typeof(Rigidbody))]
public class Carryable : MonoBehaviour
{
    //every enabled carryable in scene so player can find nearest one
    public static readonly List<Carryable> All = new List<Carryable>();

    //if dropped on floor can zombies eat it
    [SerializeField] private bool edibleOnFloor = true;

    public bool IsHeld {get; private set;}
    public TableSlot CurrentSlot {get; private set;}

    private Rigidbody body;
    private Collider[] colliders;
    private Edible edible;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        edible = GetComponent<Edible>();
    }

    private void OnEnable()
    {
        All.Add(this);
    }
    private void OnDisable()
    {
        All.Remove(this);
    }

    //frees up table slot if zombie eats it
    private void OnDestroy()
    {
        if (CurrentSlot != null) CurrentSlot.Clear(this);
    }

    //player grabs item and it sits at hold point and stops being physical
    public void PickUp(Transform holdPoint)
    {
        LeaveSlot();
        IsHeld = true;

        body.isKinematic = true;
        SetColliders(false);

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (edible != null) edible.SetOnTable(false);
    }

    public void DropOnFloor()
    {
        LeaveSlot();
        IsHeld = false;

        transform.SetParent(null);
        //stay upright
        transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
        SetColliders(true);
        body.isKinematic = false;
        body.linearVelocity = Vector3.zero;

        if (edible != null) edible.SetOnTable(edibleOnFloor);
    }

    public void PlaceOn(TableSlot slot)
    {
        LeaveSlot();
        IsHeld = false;
        CurrentSlot = slot;
        slot.Place(this);


        body.isKinematic = true;
        SetColliders(true);

        transform.SetParent(slot.SnapPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (edible != null) edible.SetOnTable(true);
    }

    private void LeaveSlot()
    {
        if (CurrentSlot != null)
        {
            CurrentSlot.Clear(this);
            CurrentSlot = null;
        }
    }

    private void SetColliders(bool value)
    {
        foreach (Collider c in colliders)
        {
            if (c != null) c.enabled = value;
        }
    }


}
