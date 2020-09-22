using UnityEngine;
using UnityEngine.Events;
using MK.Destructible;
using MK;

[System.Serializable]
public struct ProjectileData
{
    public float damage;
    public float speed;
    public float rotationSpeed;
    public float lifeTime;
    public bool destroyOnHit;

    public ProjectileData(float damage = 1, float speed = 20, float rotationSpeed = 7, float lifeTime = 2, bool destroyOnHit = true)
    {
        this.damage = damage;
        this.speed = speed;
        this.rotationSpeed = rotationSpeed;
        this.lifeTime = lifeTime;
        this.destroyOnHit = destroyOnHit;
    }
}

public class Projectile : MonoBehaviour
{
    public ProjectileData projectileData;
    public LayerMask layerMask;
    public UnityEvent onHit;
    public UnityEvent onDestroy;
    public Transform target;

    private ProjectileData defaultProjectileData;

    private void Awake()
    {
        defaultProjectileData = projectileData;
    }

    private void Start()
    {    
        if (projectileData.lifeTime >= 0)
        {
            Destroy(gameObject, projectileData.lifeTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (MKUtility.CheckIfLayerIsWithinMask(collision.transform.gameObject.layer, layerMask))
        {
            Destructible destructible = collision.gameObject.GetComponent<Destructible>();
            if (destructible)
            {
                destructible.Hurt(projectileData.damage);
            }

            onHit.Invoke();

            if (projectileData.destroyOnHit)
            {
                onDestroy.Invoke();
                Destroy(gameObject);            
            }          
        }
    }

    public void ResetProjectile()
    {
        projectileData = defaultProjectileData;
        target = null;
    }
}