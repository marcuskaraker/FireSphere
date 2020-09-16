using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject target;
    public GameObject cursor;

    private Vector3 offset;
    private Vector3 startPos;

    public Camera MainCamera { get; private set; }
    public GameObject Target
    {
        get { return target; }
        set
        {
            target = value;
            if (target == null) return;
            offset = transform.position - target.transform.position;
        }
    }

    private void Awake()
    {
        startPos = transform.position;
        Target = target;
        MainCamera = GetComponentInChildren<Camera>();
    }

    private void LateUpdate()
    {
        if (target)
        {
            transform.position = target.transform.position + offset;
        }

        if (cursor)
        {
            Vector3 mousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0f;
            cursor.transform.position = mousePos;
        }        
    }

    public void ResetPos()
    {
        transform.position = startPos;
    }
}
