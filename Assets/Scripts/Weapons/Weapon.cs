using UnityEngine;
using MK.Audio;
using MK;

public enum BarrelFireMode { FireAll, TurnFire }
public enum FireMode { Single, Automatic, Burst }

//[CreateAssetMenu(fileName = "Weapon_(WEAPON_NAME)", menuName = "Weapon/(WEAPON_NAME)")]
public abstract class Weapon : ScriptableObject
{
    [Header("Weapon Settings")]
    public Sprite weaponIcon;
    public Color weaponIconColor = Color.white;
    public Pickup weaponDropPickup;
    public BarrelFireMode barrelFireMode;
    public FireMode fireMode;

    public ProjectileData projectileData;

    [Header("Audio")]
    public AudioClipGroup fireSounds;
    public float fireSoundVolume = 1f;
    public MinMax pitchVariation = new MinMax(0.9f, 1.1f);

    [Header("Stats")]
    [Space]
    [Tooltip("The durability of the weapon. If negative, the durability is infinite.")]
    public float durability = -1;
    public float range = 10f;
    public float fireRate = 0.1f;
    public float clipSize = 50f;
    public float recoil = 0.5f;

    public abstract bool Fire(Shooter shooter);

    public virtual void Reload(Shooter shooter)
    {
        shooter.currentClipSize = clipSize;
    }

    public void PlayFireSound(Vector3 position, float volume)
    {
        AudioManager.PlayOneShot(fireSounds.GetClip(), position, volume, pitchVariation, GameManager.Instance.AudioData.audioSpatialBlend);
    }
}
