using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public GameObject cursor;

    private Vector3 offset;

    private Camera mainCamera;

    private void Awake()
    {
        if (target)
        {
            offset = transform.position - target.transform.position;
        }

        mainCamera = GetComponentInChildren<Camera>();
    }

    private void LateUpdate()
    {
        if (target)
        {
            transform.position = target.transform.position + offset;
        }

        if (cursor)
        {
            Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            cursor.transform.position = mousePos;
        }        
    }
}
