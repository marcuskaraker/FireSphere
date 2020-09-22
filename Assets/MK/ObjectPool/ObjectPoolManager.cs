using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.XR;

namespace MK
{
    public class ObjectPoolManager : MonoBehaviorSingleton<ObjectPoolManager>
    {
        Dictionary<int, ObjectPool> poolDictionary = new Dictionary<int, ObjectPool>();
        Dictionary<int, ObjectInstance> gameobjectToObjectInstance = new Dictionary<int, ObjectInstance>();

        private void Awake() => RegisterSingleton();

        /// <summary>
        /// Creates a new pool with the given pool size.
        /// </summary>
        public static Component[] CreatePool<T>(T prefab, int poolSize) where T : Component
        {
            int poolID = prefab.gameObject.GetInstanceID();

            GameObject poolParent = new GameObject("ObjectPool(" + prefab.name + ")");
            poolParent.transform.parent = Instance.transform;

            if (!Instance.poolDictionary.ContainsKey(poolID))
            {
                Instance.poolDictionary.Add(poolID, new ObjectPool(poolID, poolParent.transform));

                for (int i = 0; i < poolSize; i++)
                {
                    AddNewObjectToPool(prefab, poolID, true);
                }

                return Instance.poolDictionary[poolID].ObjectsInQueue;
            }

            return null;
        }

        /// <summary>
        /// Spawns (activates) an object from the pool the prefab belongs to. If the pool doesn't exist, it will be created.
        /// </summary>
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, int newPoolSize = 1)
        {
            return Spawn(prefab.transform, position, rotation, newPoolSize).gameObject;
        }

        /// <summary>
        /// Spawns (activates) an object from the pool the prefab belongs to. If the pool doesn't exist, it will be created.
        /// </summary>
        public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, int newPoolSize = 1) where T : Component
        {
            int poolKey = prefab.gameObject.GetInstanceID();

            if (Instance.poolDictionary.ContainsKey(poolKey))
            {
                if (Instance.poolDictionary[poolKey].queue.Count <= 0)
                {
                    // The pool is empty, add a new object to the pool.
                    ObjectInstance objectToSpawn = CreateNewObjectInstance(prefab, poolKey, true);
                    objectToSpawn.Spawn(position, rotation);
                    return objectToSpawn.Component as T;
                }
                else
                {
                    // The pool is not empty, grab a inactive object from the queue.
                    ObjectInstance objectToSpawn = Instance.poolDictionary[poolKey].queue.Dequeue();
                    objectToSpawn.Spawn(position, rotation);
                    return objectToSpawn.Component as T;
                }
            }
            else
            {
                // The pool doesn't exist. Create one.
                CreatePool(prefab, newPoolSize);

                if (Instance.poolDictionary.ContainsKey(poolKey))
                {
                    ObjectInstance objectToSpawn = Instance.poolDictionary[poolKey].queue.Dequeue();
                    objectToSpawn.Spawn(position, rotation);
                    return objectToSpawn.Component as T;
                }
                else
                {
                    Debug.LogError("Failed to create pool on spawn!");
                    return null;
                }
            }
        }

        /// <summary>
        /// Destroys (deactivates) the object from the pool the object belongs to. If the pool doesn't exist, it will be destroyed normally.
        /// </summary>
        public static void DeSpawn(GameObject objectToDestroy, float time = 0f)
        {
            if (time > 0)
            {
                Instance.StartCoroutine(Instance.DoDeSpawnAfterTime(objectToDestroy, time));
            }
            else
            {
                DeSpawnObject(objectToDestroy);
            }
        }

        private IEnumerator DoDeSpawnAfterTime(GameObject objectToDestroy, float time)
        {
            yield return new WaitForSeconds(time);
            DeSpawnObject(objectToDestroy);
        }

        private static void DeSpawnObject(GameObject objectToDestroy)
        {
            int objectID = objectToDestroy.GetInstanceID();

            ObjectInstance objectInstance;
            if (Instance.gameobjectToObjectInstance.TryGetValue(objectID, out objectInstance))
            {
                ObjectPool objectPool;
                if (Instance.poolDictionary.TryGetValue(objectInstance.PoolID, out objectPool))
                {
                    // The object was in a pool. Deactivate.
                    objectInstance.Destroy();
                    objectPool.queue.Enqueue(objectInstance);
                }
                else
                {
                    // The object has a objectinstance reference but does not have a pool.
                    Instance.gameobjectToObjectInstance.Remove(objectID);
                    GameObject.Destroy(objectToDestroy);
                    Debug.LogWarning("Destroyed an object without a pool but with an objectinstance.");
                }
            }
            else
            {
                // The object was not spawned through a pool. Destroy the object normally.
                GameObject.Destroy(objectToDestroy);
                Debug.Log("Destroyed an object without a pool.");
            }
        }

        private static void AddNewObjectToPool<T>(T prefab, int poolKey, bool overrideContainsCheck = false) where T : Component
        {
            if (overrideContainsCheck || Instance.poolDictionary.ContainsKey(poolKey))
            {
                ObjectInstance spawnedObject = CreateNewObjectInstance(prefab, poolKey, true);

                ObjectPool pool = Instance.poolDictionary[poolKey];
                pool.queue.Enqueue(spawnedObject);
            }
        }

        private static ObjectInstance CreateNewObjectInstance<T>(T prefab, int poolKey, bool overrideContainsCheck = false) where T : Component
        {
            if (overrideContainsCheck || Instance.poolDictionary.ContainsKey(poolKey))
            {
                ObjectPool pool = Instance.poolDictionary[poolKey];
                T spawnedGameObject = Instantiate(prefab);
                ObjectInstance spawnedObject = new ObjectInstance(spawnedGameObject, pool.PoolID);

                spawnedObject.SetParent(pool.PoolParent);
                Instance.gameobjectToObjectInstance.Add(spawnedGameObject.gameObject.GetInstanceID(), spawnedObject);

                return spawnedObject;
            }

            Debug.LogError("Tried to create ObjectInstance for a pool that does not exist!", Instance);
            return null;
        }

        public class ObjectPool
        {
            public Queue<ObjectInstance> queue;

            public int PoolID { get; private set; }
            public Transform PoolParent { get; private set; }

            public Component[] ObjectsInQueue
            { 
                get 
                {
                    Component[] result = new Component[queue.Count];
                    int index = 0;
                    foreach (ObjectInstance objectInstance in queue)
                    {
                        result[index] = objectInstance.Component;
                        index++;
                    }

                    return result;
                } 
            }

            public ObjectPool(int poolID, Transform poolParent)
            {
                queue = new Queue<ObjectInstance>();

                PoolID = poolID;
                PoolParent = poolParent;
            }
        }

        public class ObjectInstance
        {
            bool hasPoolObjectComponent;
            IPoolObject poolObject;

            public Component Component { get; private set; }
            public GameObject GameObject { get; private set; }
            public Transform Transform { get; private set; }

            public int PoolID { get; private set; }
            public bool IsActive { get; private set; }

            public ObjectInstance(Component objectInstance, int poolID)
            {
                Component = objectInstance;
                GameObject = objectInstance.gameObject;
                Transform = GameObject.transform;
                GameObject.SetActive(false);

                PoolID = poolID;

                IPoolObject poolObject = GameObject.GetComponent<IPoolObject>();
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
                Transform.position = position;
                Transform.rotation = rotation;
            }

            public void Destroy()
            {
                if (hasPoolObjectComponent) poolObject.OnDeSpawn();

                SetActive(false);
                Transform.position = Vector3.zero;
                Transform.rotation = Quaternion.identity;
            }

            public void SetParent(Transform parent)
            {
                Transform.parent = parent;
            }

            private void SetActive(bool value)
            {
                IsActive = value;
                if (GameObject == null)
                {
                    Debug.Log("NULL: SetActive(" + value + ")");
                }
                GameObject.SetActive(value);
            }
        }
    }
}