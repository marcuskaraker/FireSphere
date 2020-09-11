using EZCameraShake;
using UnityEngine;

public class ObjectUtilities : MonoBehaviour
{
    public void ShakeSmall()
    {
        CameraShaker.Instance.ShakeOnce(2f, 2f, 0, 0.1f);
    }

    public void ShakeMedium()
    {
        CameraShaker.Instance.ShakeOnce(4f, 4f, 0, 0.1f);
    }

    public void ShakeBig()
    {
        CameraShaker.Instance.ShakeOnce(8f, 8f, 0, 0.1f);
    }

    public void Shake(float size)
    {
        CameraShaker.Instance.ShakeOnce(size, size, 0, 0.1f);
    }
}
