using UnityEngine;
using UnityEngine.Events;
using MK.Destructible;
using MK;

[System.Serializable]
public class ProjectileData
{
    public float damage = 1f;
    public float speed = 20f;
    public float lifeTime = 2f;
    public bool destroyOnHit = true;
}

public class Projectile : MonoBehaviour
{
    public ProjectileData projectileData;

    public LayerMask layerMask;

    public UnityEvent onHit;

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
                Destroy(gameObject);
            }          
        }
    }
}