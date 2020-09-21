﻿using MK;
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
    public Vector3 spawningPadding = new Vector3(10, 10, 10);

    [Header("World Objects Spawning")]
    public GameObject[] worldObjects;
    public float worldObjectSpawnChance = 0.1f;

    [Header("Pickups")]
    public PickupInteractable pickupPrefab;
    public Pickup[] allPickups;

    [Header("WorldObject Clear Radius")]
    public float playerClearRadius = 3f;
    public float cruiserClearRadius = 6f;

    [Header("Audio")]
    public AudioClip mainTheme;
    public float mainThemeVolume = 0.5f;
    [Space]
    public AudioClip engineAudio;
    public float engineVolume = 0.1f;
    [Space]
    public AudioClip shieldAudio;
    public float shieldVolume = 0.1f;

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
