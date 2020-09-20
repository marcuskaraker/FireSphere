using System.Collections.Generic;
using UnityEngine;

namespace MK
{
    public class ObjectPool : MonoBehaviorSingleton<ObjectPool>
    {
        Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();
        Dictionary<int, ObjectInstance> gameobjectToObjectInstance = new Dictionary<int, ObjectInstance>();

        private void Awake() => RegisterSingleton();

        public static void CreatePool(GameObject prefab, int poolSize)
        {
            int poolID = prefab.GetInstanceID();

            GameObject poolParent = new GameObject("ObjectPool(" + prefab.name + ")");
            poolParent.transform.parent = Instance.transform;

            if (!Instance.poolDictionary.ContainsKey(poolID))
            {
                Instance.poolDictionary.Add(poolID, new Queue<ObjectInstance>());

                for (int i = 0; i < poolSize; i++)
                {
                    GameObject spawnedGameObject = Instantiate(prefab);
                    ObjectInstance spawnedObject = new ObjectInstance(spawnedGameObject, poolID);
                    Instance.poolDictionary[poolID].Enqueue(spawnedObject);
                    spawnedObject.SetParent(poolParent.transform);

                    Instance.gameobjectToObjectInstance.Add(spawnedGameObject.GetInstanceID(), spawnedObject);
                }
            }
        }

        public static void Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            int poolKey = prefab.GetInstanceID();

            if (Instance.poolDictionary.ContainsKey(poolKey))
            {
                ObjectInstance objectToSpawn = Instance.poolDictionary[poolKey].Dequeue();
                objectToSpawn.Spawn(position, rotation);
            }
        }

        public static void Destroy(GameObject objectToDestroy)
        {
            int objectID = objectToDestroy.GetInstanceID();

            ObjectInstance objectInstance;
            if (Instance.gameobjectToObjectInstance.TryGetValue(objectID, out objectInstance))
            {
                Queue<ObjectInstance> objectPool;
                if (Instance.poolDictionary.TryGetValue(objectInstance.PoolID, out objectPool))
                {
                    // The object was in a pool. Deactivate.
                    objectInstance.Destroy();
                    objectPool.Enqueue(objectInstance);
                }
                else
                {
                    // The object has a objectinstance reference but does not have a pool.
                    Instance.gameobjectToObjectInstance.Remove(objectID);
                    GameObject.Destroy(objectToDestroy);
                }               
            }
            else
            {
                // The object was not spawned through a pool. Destroy the object normally.
                GameObject.Destroy(objectToDestroy);
            }
        }

        public class ObjectInstance
        {
            GameObject gameObject;
            Transform transform;

            bool hasPoolObjectComponent;
            IPoolObject poolObject;

            public int PoolID { get; private set; }
            public bool IsActive { get; private set; }

            public ObjectInstance(GameObject objectInstance, int poolID)
            {
                gameObject = objectInstance;
                transform = gameObject.transform;
                gameObject.SetActive(false);

                PoolID = poolID;

                IPoolObject poolObject = gameObject.GetComponent<IPoolObject>();
                if (poolObject != null)
                {
                    hasPoolObjectComponent = true;
                    this.poolObject = poolObject;
                }
            }

            public void Spawn(Vector3 position, Quaternion rotation)
            {
                if (hasPoolObjectComponent) poolObject.OnSpawn();

                SetActive(true);
                transform.position = position;
                transform.rotation = rotation;
            }

            public void Destroy()
            {
                if (hasPoolObjectComponent) poolObject.OnDestroy();

                SetActive(false);
            }

            public void SetParent(Transform parent)
            {
                transform.parent = parent;
            }

            private void SetActive(bool value)
            {
                IsActive = value;
                gameObject.SetActive(value);
            }
        }
    }
}