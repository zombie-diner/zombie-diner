using System.Collections.Generic;
using UnityEngine;

//crate with infinite food. press E to grab from crate with empty hands
public class FoodCrate : MonoBehaviour
{
    // Every enabled crate in the scene, so the player can find the nearest one
    public static readonly List<FoodCrate> All = new List<FoodCrate>();

    [Tooltip("The food item this crate hands out. It needs a Carryable component (and an Edible so zombies can eat it).")]
    [SerializeField] private GameObject foodPrefab;

    private void OnEnable()
    {
        All.Add(this);
    }

    private void OnDisable()
    {
        All.Remove(this);
    }

    public Carryable Spawn(Transform spawnPoint)
    {
        if (foodPrefab == null)
        {
            Debug.LogWarning(name + ": no food prefab assigned.");
            return null;
        }

        GameObject food = Instantiate(foodPrefab, spawnPoint.position, spawnPoint.rotation);
        Carryable carryable = food.GetComponent<Carryable>();

        if (carryable == null)
        {
            Debug.LogError(name + ": the food prefab needs a Carryable component.");
            Destroy(food);
            return null;
        }
        return carryable;
    }
}
