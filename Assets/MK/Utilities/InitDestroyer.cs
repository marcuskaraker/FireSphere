using UnityEngine;

namespace MK
{
    public class InitDestroyer : MonoBehaviour
    {
        [SerializeField] float timeToDestroy = 2f;
        [SerializeField] bool objectPool;        

        private void Awake()
        {
            if (objectPool)
            {
                ObjectPoolManager.DeSpawn(gameObject, timeToDestroy);
            }
            else
            {
                Destroy(gameObject, timeToDestroy);
            }
        }
    }
}

