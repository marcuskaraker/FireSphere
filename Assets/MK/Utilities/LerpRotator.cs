using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpRotator : MonoBehaviour
{
    public float targetRotation;
    public float speed = 5;

    private void Update()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * speed);
    }
}
