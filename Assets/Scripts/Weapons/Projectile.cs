using UnityEngine;
using UnityEngine.Events;
using MK.Destructible;
using MK;

public class Projectile : MonoBehaviour
{
    public float damage = 1f;
    public float speed = 1f;
    public float lifeTime = 2f;
    public bool destroyOnHit = true;

    public LayerMask layerMask;

    public UnityEvent onHit;

    private void Start()
    {
        if (lifeTime >= 0)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (MKUtility.CheckIfLayerIsWithinMask(collision.transform.gameObject.layer, layerMask))
        {
            Destructible destructible = collision.gameObject.GetComponent<Destructible>();
            if (destructible)
            {
                destructible.Hurt(damage);
            }

            onHit.Invoke();

            if (destroyOnHit)
            {
                Destroy(gameObject);
            }          
        }
    }
}