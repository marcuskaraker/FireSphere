using UnityEngine;

namespace MK
{
    public class Instantiator : MonoBehaviour
    {
        public GameObject[] objectsToSpawn;
        public int newPoolSize = 100;

        public void SpawnObject(GameObject go)
        {
            Instantiate(go, transform.position, Quaternion.identity);
        }

        public void SpawnObjectFromPool(GameObject go)
        {
            ObjectPoolManager.Spawn(go, transform.position, Quaternion.identity, newPoolSize);
        }

        public void SpawnRandomObjectFromList()
        {
            Instantiate(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)], transform.position, Quaternion.identity);
        }

        public void SpawnRandomObjectsFromList(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnRandomObjectFromList();
            }
        }

        public void SpawnObjectOnChance(float percentage)
        {
            if (Random.Range(0f, 1f) > percentage) return;

            SpawnRandomObjectFromList();
        }
    }
}
