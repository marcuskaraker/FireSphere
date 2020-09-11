using UnityEngine;
using EZCameraShake;

public class CameraShakeUtility : MonoBehaviour
{
    public void CameraShake01(float time)
    {
        CameraShaker.Instance.ShakeOnce(2f, 3f, 0f, time);
    }

    public void CameraShake02(float time)
    {
        CameraShaker.Instance.ShakeOnce(4f, 5f, 0f, time);
    }

    public void CameraShake03(float time)
    {
        CameraShaker.Instance.ShakeOnce(5f, 6f, 0f, time);
    }

    public void CameraShake04(float time)
    {
        CameraShaker.Instance.ShakeOnce(6f, 7f, 0f, time);
    }
}
