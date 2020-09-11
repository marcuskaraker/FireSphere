using UnityEngine;
using UnityEngine.Events;

namespace MK.Destructible
{
    public class Destructible : MonoBehaviour
    {
        public float health = 100f;
        public float maxHealth = 100f;

        public bool destroyOnDeath = true;

        public UnityEvent onHurt;
        public UnityEvent onHeal;
        public UnityEvent onDeath;

        public void Hurt(float damage)
        {
            health -= damage;
            health = Mathf.Clamp(health, 0f, maxHealth);

            if (damage >= 0)
            {
                onHurt.Invoke();
            }
            else
            {
                onHeal.Invoke();
            }

            if (health <= 0f)
            {
                Die();
            }
        }

        public void Die()
        {
            onDeath.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
        }
        private void OnValidate()
        {
            health = Mathf.Clamp(health, 0, maxHealth);
        }
    }
}
