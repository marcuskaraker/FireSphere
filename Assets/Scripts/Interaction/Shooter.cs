using System;
using UnityEngine;
using UnityEngine.Events;
using MK;

public class Shooter : MonoBehaviour
{
    public Weapon[] loadout;
    [NonSerialized] public float[] durability;
    public Transform[] firePositions;
    public bool recoilRigidbody = true;

    public float currentClipSize;

    [System.NonSerialized] public int firePointIndex;
    [System.NonSerialized] public float fireTimer;

    public UnityEvent onShoot;

    private Rigidbody2D rb2D;
    private int currentWeaponIndex;

    public float LatestRelativeVelocity { get; private set; }

    public int CurrentWeaponIndex
    {
        get { return currentWeaponIndex; }
        set { currentWeaponIndex = MKUtility.NegativeModulo(value, loadout.Length); }
    }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        durability = new float[loadout.Length];

        EquipWeapon(loadout[CurrentWeaponIndex], 1, false);
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;
    }

    public bool Shoot(float relativeVelocity = 0f)
    {
        if (WeaponNullCheck()) return false;

        Weapon currentWeapon = loadout[CurrentWeaponIndex];

        if (fireTimer < currentWeapon.fireRate) return false;
        if (currentWeapon.clipSize >= 0 && currentClipSize <= 0) return false;

        // Fire
        LatestRelativeVelocity = relativeVelocity;
        currentWeapon.Fire(this);

        // Recoil
        if (recoilRigidbody)
        {
            Recoil();
        }

        fireTimer = 0f;
        onShoot.Invoke();

        // Durability
        if (currentWeapon.durability >= 0)
        {
            durability[CurrentWeaponIndex] = Mathf.Max(durability[CurrentWeaponIndex] - 1, 0);

            if (durability[CurrentWeaponIndex] <= 0)
            {
                EquipWeapon(null, 0, false);
            }
        }

        return true;
    }

    public bool Reload()
    {
        if (WeaponNullCheck()) return false;

        loadout[CurrentWeaponIndex].Reload(this);
        return true;
    }

    /// <summary>
    /// Equips the set weapon and sets its durability. Durability is a value from 0-1.
    /// </summary>
    public void EquipWeapon(Weapon weapon, float durability = 1f, bool dropWeapon = true)
    {
        if (dropWeapon && loadout[CurrentWeaponIndex] != null && loadout[CurrentWeaponIndex].weaponDropPickup != null)
        {
            PickupInteractable spawnedPickup = GameManager.Instance.SpawnPickup(
                loadout[CurrentWeaponIndex].weaponDropPickup, 
                transform.position
            );

            if (spawnedPickup.instancePickupData is InstanceWeaponPickupData instanceWeaponPickupData)
            {
                instanceWeaponPickupData.durability = GetDurabilityPercentageOfWeapon(CurrentWeaponIndex);
            }
        }

        loadout[CurrentWeaponIndex] = weapon;
        this.durability[CurrentWeaponIndex] = weapon != null ? Mathf.Clamp01(durability) * weapon.durability : 0;

        if (weapon == null) return;

        currentClipSize = weapon.clipSize;
    }

    public bool CompareWeaponFireMode(FireMode fireMode)
    {
        if (WeaponNullCheck()) return false;
        return loadout[CurrentWeaponIndex].fireMode == fireMode;
    }

    public void Recoil()
    {
        if (rb2D == null || WeaponNullCheck()) return;

        rb2D.AddForce(-transform.right * loadout[CurrentWeaponIndex].recoil, ForceMode2D.Impulse);
    }

    private bool WeaponNullCheck()
    {
        return (loadout == null || loadout.Length < CurrentWeaponIndex || loadout[CurrentWeaponIndex] == null);
    }

    public float GetDurabilityPercentageOfWeapon(int index)
    {
        return loadout[index] != null ? (durability[index] / loadout[index].durability) : 0;
    }
}