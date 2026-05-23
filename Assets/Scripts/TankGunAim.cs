using UnityEngine;

public class TankGunAim : MonoBehaviour
{
    [SerializeField] 
    private float pitchSpeed = 30f;

    [SerializeField]
    private float minPitch = -10f;

    [SerializeField]
    private float maxPitch = 30f;

    private float currentAngle;

    private void Awake()
    {
        // 포신의 로컬 X 회전값
        currentAngle = transform.localEulerAngles.x;  

        // 180도 이상 초과 값 음수 각도로 보정.
        // ~180~180도 형태로 처리하도록 
        if (currentAngle > 180f) currentAngle -= 360f;

        // 시작 각도에 따른 제한
        currentAngle = Mathf.Clamp(currentAngle, minPitch, maxPitch);

        // 로컬 회전 값 명시
        transform.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);
    }

    void Update()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.UpArrow)) input -= 1f;
        if (Input.GetKey(KeyCode.DownArrow)) input += 1f;

        currentAngle += input * pitchSpeed * Time.deltaTime;

        // 최소, 최대 각도 제한.
        currentAngle = Mathf.Clamp(currentAngle, minPitch, maxPitch);

        // 로컬 회전 값 명시
        transform.localRotation = Quaternion.Euler(currentAngle, 0f, 0f);
    }
}
