using UnityEngine;

public class InitDestroyer : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 2f;

    private void Awake()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
