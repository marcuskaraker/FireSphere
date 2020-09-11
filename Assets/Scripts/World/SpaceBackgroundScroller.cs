using UnityEngine;

public class SpaceBackgroundScroller : MonoBehaviour
{
    public Transform followTarget;
    public float scrollSpeedMultiplier = 0.25f;

    private MeshRenderer meshRenderer;
    private Vector3 offset;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (followTarget)
        {
            offset = transform.position - followTarget.transform.position;
        }
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
}
