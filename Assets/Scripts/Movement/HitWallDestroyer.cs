using UnityEngine;
using MK.Destructible;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Destructible))]
public class HitWallDestroyer : MonoBehaviour
{
    public float velocitySquaredThreshold = 1f;
    public float collisionDamage = 10f;
    public bool instaDeathOnOtherDestructible = true;

    private Rigidbody2D rb2D;
    private Destructible destructible;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        destructible = GetComponent<Destructible>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destructible otherDestructible = collision.transform.GetComponent<Destructible>();

        if ((instaDeathOnOtherDestructible && otherDestructible) || (rb2D.velocity.sqrMagnitude > velocitySquaredThreshold))
        {
            if (otherDestructible)
            {
                otherDestructible.Hurt(collisionDamage);
            }

            destructible.Hurt(destructible.health);
        }
    }
}
