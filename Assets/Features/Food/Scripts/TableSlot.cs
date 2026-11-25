using System.Collections.Generic;
using UnityEngine;

public class TableSlot : MonoBehaviour
{
    //every enabled slot in scene
    public static readonly List<TableSlot> All = new List<TableSlot>();

    [Tooltip("Where the item sits. Leave empty to use this object's own position.")]
    [SerializeField] private Transform snapPoint;

    public Carryable Item {get; private set;}

    public bool IsFree
    {
        get { return Item == null; }
    }

    public Transform SnapPoint
    {
        get { return snapPoint != null ? snapPoint : transform; }
    }

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    //called by Carryable.PlaceOn
    public void Place(Carryable item)
    {
        Item = item;
    }

    //called when item is picked up or destroyed
    public void Clear(Carryable item)
    {
        if (Item == item) Item = null;
    }  
}
