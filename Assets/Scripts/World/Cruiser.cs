using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Cruiser : MonoBehaviour
{
    public GameObject target;
    public bool findPlayerTargetOnStart = true;

    public float speed = 1f;

    public Transform explosionPointParent;
    public GameObject explosionEffect;

    private int health;
    private EnemyController[] turrets;

    public UnityEvent onShipSpawn;
    public UnityEvent onShipExplosion;
    public UnityEvent onShipDestroy;

    private const float EXPLOSION_WAIT_TIME_MIN = 0.1f;
    private const float EXPLOSION_WAIT_TIME_MAX = 0.75f;

    private void Start()
    {
        turrets = transform.GetComponentsInChildren<EnemyController>();

        if (findPlayerTargetOnStart)
        {
            target = GameManager.Instance.Player;
        }

        if (target)
        {
            foreach (EnemyController turret in turrets)
            {
                turret.chaseTarget = target.transform;
            }
        }    
        
        health = turrets.Length;

        onShipSpawn.Invoke();
    }

    public void DecreaseHealth()
    {
        health--;
        health = Mathf.Max(0, health);

        if (health <= 0)
        {
            DestroyCruiser();
        }
    }

    public void DestroyCruiser()
    {
        StartCoroutine(DoDestroyCruiser());
    }

    private IEnumerator DoDestroyCruiser()
    {
        for (int i = 0; i < explosionPointParent.childCount; i++)
        {
            Vector2 pos = explosionPointParent.GetChild(i).position;
            GameObject spawnedExplosion = Instantiate(explosionEffect, pos, Quaternion.identity);

            onShipExplosion.Invoke();

            if (i == (explosionPointParent.childCount-1))
            {
                spawnedExplosion.transform.localScale *= 2;

                for (int j = 0; j < explosionPointParent.childCount - 1; j++)
                {
                    Instantiate(explosionEffect, explosionPointParent.GetChild(j).position, Quaternion.identity);
                }
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(EXPLOSION_WAIT_TIME_MIN, EXPLOSION_WAIT_TIME_MAX));
            }            
        }

        onShipDestroy.Invoke();
        Destroy(gameObject);
    }
}
