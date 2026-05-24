using UnityEngine;

public class TankRecoverOld : MonoBehaviour
{
    [SerializeField] private float upsideDownCheckTime = 1.5f; // 뒤집힘 판정 후 복구까지 기다리는 시간
    [SerializeField] private float recoverySpeed = 3f;         // 회전 보간 속도

    private Rigidbody rb;
    private float upsideDownTimer;
    private bool isRecovering;

    private void Awake()
    {
        // Rigidbody 캐싱
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 탱크의 위쪽 방향과 월드 위쪽 방향을 비교해 뒤집힘 여부 판단
        bool isUpsideDown = Vector3.Dot(transform.up, Vector3.up) < 0.3f;

        if (isUpsideDown)
        {
            // 뒤집힌 시간이 일정 시간 이상 지속되면 복구 시작
            upsideDownTimer += Time.deltaTime;

            if (upsideDownTimer >= upsideDownCheckTime)
            {
                isRecovering = true;
            }
        }
        else
        {
            // 정상 자세면 복구 상태 초기화
            upsideDownTimer = 0f;
            isRecovering = false;
        }
    }

    private void FixedUpdate()
    {
        // 복구 중이 아니면 처리하지 않음
        if (isRecovering == false) return;

        // 복구 중 과도한 이동/회전 줄이기
        rb.linearVelocity *= 0.5f;
        rb.angularVelocity *= 0.5f;

        // 현재 Y 회전은 유지하고 X/Z 기울기만 0으로 되돌림
        Quaternion targetRotation = Quaternion.Euler(0f,transform.eulerAngles.y, 0f);

        // 현재 회전에서 목표 회전으로 부드럽게 보간
        Quaternion nextRotation = Quaternion.Slerp(rb.rotation,targetRotation,recoverySpeed * Time.fixedDeltaTime);

        // Rigidbody 회전 적용
        rb.MoveRotation(nextRotation);
    }
}