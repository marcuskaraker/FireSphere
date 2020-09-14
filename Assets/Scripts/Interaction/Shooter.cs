using System;
using UnityEngine;
using UnityEngine.Events;

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
        set { currentWeaponIndex = Mathf.Abs(value) % loadout.Length; }
    }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        durability = new float[loadout.Length];

        EquipWeapon(loadout[CurrentWeaponIndex], false);
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

        LatestRelativeVelocity = relativeVelocity;
        currentWeapon.Fire(this);

        if (currentWeapon.durability >= 0)
        {
            durability[CurrentWeaponIndex] = Mathf.Max(durability[CurrentWeaponIndex] - 1, 0);

            if (durability[CurrentWeaponIndex] <= 0)
            {
                
            }
        }

        if (recoilRigidbody)
        {
            Recoil();
        }

        fireTimer = 0f;
        onShoot.Invoke();

        return true;
    }

    public bool Reload()
    {
        if (WeaponNullCheck()) return false;

        loadout[CurrentWeaponIndex].Reload(this);
        return true;
    }

    public void EquipWeapon(Weapon weapon, bool dropWeapon = true)
    {
        if (dropWeapon && loadout[CurrentWeaponIndex] != null && loadout[CurrentWeaponIndex].weaponDropPickup != null)
        {
            GameManager.Instance.SpawnPickup(loadout[CurrentWeaponIndex].weaponDropPickup, transform.position);
        }

        loadout[CurrentWeaponIndex] = weapon;

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
}