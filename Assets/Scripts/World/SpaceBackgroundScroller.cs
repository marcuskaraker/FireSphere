using UnityEngine;

public class SpaceBackgroundScroller : MonoBehaviour
{
    public Transform followTarget;
    public float scrollSpeedMultiplier = 0.25f;

    private MeshRenderer meshRenderer;
    private Vector3 offset;

    private Vector3 startPos;

    public Transform Target
    {
        get { return followTarget; }
        set
        {
            followTarget = value;
            if (followTarget == null) return;
            offset = transform.position - followTarget.transform.position;
        }
    }

    private void Awake()
    {
        startPos = transform.position;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Scroll(Vector2 direction)
    {
        direction *= scrollSpeedMultiplier;

        Vector2 currentOffsetMainTex = meshRenderer.material.GetTextureOffset("_MainTex");
        meshRenderer.material.SetTextureOffset("_MainTex", currentOffsetMainTex + direction);

        Vector2 currentOffsetMidTex = meshRenderer.material.GetTextureOffset("_MidTex");
        meshRenderer.material.SetTextureOffset("_MidTex", currentOffsetMidTex + direction / 2);

        Vector2 currentOffsetBackTex = meshRenderer.material.GetTextureOffset("_BackTex");
        meshRenderer.material.SetTextureOffset("_BackTex", currentOffsetBackTex + direction / 4);
    }

    private void LateUpdate()
    {
        if (followTarget)
        {
            transform.position = followTarget.transform.position + offset;
        }
    }

    public void ResetPos()
    {
        transform.position = startPos;
    }
}
