using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] GameObject target;

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
    }

    public void ResetPos()
    {
        transform.position = startPos;
    }
}
