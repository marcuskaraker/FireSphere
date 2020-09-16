using MK;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data")]
public class GameData : ScriptableObject
{
    [Header("Player")]
    public GameObject playerPrefab;
    public Shield playerShieldPrefab;

    [Header("Cruiser Spawning")]
    public Cruiser cruiserPrefab;
    public MinMax cruiserMinMaxSpawnInterval = new MinMax(10f, 60f);

    [Header("World Objects Spawning")]
    public GameObject[] worldObjects;
    public float worldObjectSpawnChance = 0.1f;

    [Header("Pickups")]
    public PickupInteractable pickupPrefab;
    public Pickup[] allPickups;

    Dictionary<string, Pickup> nameToPickupMap = new Dictionary<string, Pickup>();

    public void InitData()
    {
        for (int i = 0; i < allPickups.Length; i++)
        {
            nameToPickupMap.Add(allPickups[i].pickupName.ToLower(), allPickups[i]);
        }
    }

    public Pickup GetPickup(string name)
    {
        Pickup returnValue = null;

        if (nameToPickupMap.TryGetValue(name.ToLower(), out returnValue))
        {
            return returnValue;
        }

        Debug.LogError("Tried to get a pickup that does not exist with key: " + name);
        return null;
    }
}
