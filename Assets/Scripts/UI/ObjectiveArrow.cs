using UnityEngine;

public class ObjectiveArrow : MonoBehaviour
{
    public Transform target;
    public RectTransform arrow;
    public Canvas canvas;
    public float borderSize = 100f;

    private RectTransform canvasRectTransform;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (target == null)
        {
            arrow.gameObject.SetActive(false);
            return; 
        }

        // Rotate Arrow
        Vector3 dir = (target.position - mainCamera.transform.position);
        float angle = GetAngleFromVector(dir);
        arrow.localEulerAngles = new Vector3(0, 0, angle);

        float width = Screen.width - borderSize;
        float height = Screen.height - borderSize;

        // Is target on screen
        Vector3 targetPositionScreenPoint = mainCamera.WorldToScreenPoint(target.position);
        bool targetIsOffScreen =
            targetPositionScreenPoint.x <= borderSize ||
            targetPositionScreenPoint.x >= width ||
            targetPositionScreenPoint.y <= borderSize ||
            targetPositionScreenPoint.y >= height;

        arrow.gameObject.SetActive(targetIsOffScreen);

        if (targetIsOffScreen)
        {
            Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
            if (cappedTargetScreenPosition.x <= borderSize) cappedTargetScreenPosition.x = borderSize;
            if (cappedTargetScreenPosition.x >= width) cappedTargetScreenPosition.x = width;
            if (cappedTargetScreenPosition.y <= borderSize) cappedTargetScreenPosition.y = borderSize;
            if (cappedTargetScreenPosition.y >= height) cappedTargetScreenPosition.y = height;

            Vector2 arrowPivotPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                cappedTargetScreenPosition,
                null,
                out arrowPivotPoint
            );

            arrow.anchoredPosition = arrowPivotPoint;
        }
    }

    public float GetAngleFromVector(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        return angle;
    }
}
