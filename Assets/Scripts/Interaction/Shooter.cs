using UnityEngine;
using UnityEngine.Events;

public class Shooter : MonoBehaviour
{
    public Weapon weapon;
    public Transform[] firePositions;
    public bool recoilRigidbody = true;

    public float currentClipSize;

    [System.NonSerialized] public int firePointIndex;
    [System.NonSerialized] public float fireTimer;

    public UnityEvent onShoot;

    Rigidbody2D rb2D;

    public float LatestRelativeVelocity { get; private set; }

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();

        EquipWeapon(weapon);
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;
    }

    public bool Shoot(float relativeVelocity = 0f)
    {
        if (weapon == null) return false;
        if (fireTimer < weapon.fireRate) return false;
        if (weapon.clipSize >= 0 && currentClipSize <= 0) return false;

        LatestRelativeVelocity = relativeVelocity;

        weapon.Fire(this);

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
        if (weapon == null) return false;

        weapon.Reload(this);
        return true;
    }

    public void EquipWeapon(Weapon weapon)
    {
        this.weapon = weapon;

        if (weapon == null) return;

        currentClipSize = weapon.clipSize;
    }

    public bool CompareWeaponFireMode(FireMode fireMode)
    {
        if (weapon == null) return false;
        return weapon.fireMode == fireMode;
    }

    public void Recoil()
    {
        if (rb2D == null || weapon == null) return;

        rb2D.AddForce(-transform.right * weapon.recoil, ForceMode2D.Impulse);
    }
}