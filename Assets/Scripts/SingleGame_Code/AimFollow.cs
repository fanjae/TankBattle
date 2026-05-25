using UnityEngine;

public class AimFollow : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private RectTransform aim;

    public void SetTarget(Camera camera, Transform point)
    {
        targetCamera = camera;
        muzzlePoint = point;
    }
    private void LateUpdate()
    {
        if (targetCamera == null || muzzlePoint == null || aim == null)
            return;

        Vector3 screenPos = targetCamera.WorldToScreenPoint(muzzlePoint.position); // target 카메라 기준으로 조준점을 맞춘다.

        if (screenPos.z < 0f) // 포신 위치가 카메라 뒤쪽이면 조준점을 숨기도록 처리
        {
            aim.gameObject.SetActive(false);
            return;
        }

        aim.gameObject.SetActive(true);
        aim.position = screenPos;
    }
}