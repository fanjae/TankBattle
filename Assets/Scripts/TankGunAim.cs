using UnityEngine;

public class TankGunAim : MonoBehaviour
{
    [SerializeField] 
    private float pitchSpeed = 30f;

    [SerializeField]
    private float minPitch = -10;

    [SerializeField]
    private float maxPitch = 30f;

    private float currentAngle;
    
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
