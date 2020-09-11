using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpScaler : MonoBehaviour
{
    public Vector3 targetScale = new Vector3(1, 1, 1);
    public float speed = 5f;
    public float destroyTime = -1;

    private void Start()
    {
        if (destroyTime > 0)
        {
            Destroy(this, destroyTime);
        }
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }
}
