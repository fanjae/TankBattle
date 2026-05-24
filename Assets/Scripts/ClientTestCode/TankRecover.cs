using UnityEngine;

public class TankRecover : MonoBehaviour
{
    [SerializeField] private float upsideDownCheckTime = 1.5f;  // 뒤집힘 체크
    [SerializeField] private float recoverySpeed = 3f; // 회전 보간 속도
    [SerializeField] private float recoveryBounceForce = 7f;
    [SerializeField] private float recoveryTorqueForce = 5f;

    private Rigidbody rb;
    private float upsideDownTimer;
    private bool isRecovering; 
    private bool hasBounced;

    private void Awake()
    {
        // Rigidbody 캐싱
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 
        bool isUpsideDown = Vector3.Dot(transform.up, Vector3.up) < 0.3f;

        if (isUpsideDown)
        {
            // 뒤집힌 상황인지 확인(일정 시간이 넘으면 복구를 진행)
            upsideDownTimer += Time.deltaTime;

            if (upsideDownTimer >= upsideDownCheckTime)
            {
                isRecovering = true;
            }
        }
        else
        {
            // 복구 상태 초기화
            upsideDownTimer = 0f;
            isRecovering = false;
            hasBounced = false;
        }
    }

    private void FixedUpdate()
    {
        // 복구 중일때만 물리처리 하도록 한다.
        if (isRecovering == false) return;

        if (hasBounced == false)
        {
            // 복구 시작시 한번만 위로 튕기게 만듦. (바닥 끼임을 완화한다.)
            rb.AddForce(Vector3.up * recoveryBounceForce, ForceMode.Impulse);

            // 탱크를 회전 시킬때 회전 시키는 힘을 준다. 자세 복구를 보조한다.
            rb.AddTorque(transform.right * recoveryTorqueForce, ForceMode.Impulse);
            hasBounced = true;
        }

        rb.linearVelocity *= 0.95f;
        rb.angularVelocity *= 0.95f;

        // 현재 바라보는 방향 기준으로 복구 시키기
        Vector3 forward = transform.forward;

        // 방향은 수평만 사용. 
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
        {
            forward = transform.right;
            forward.y = 0f;
        }

        // 바라보는 방향은 유지한 상태. 위쪽은 월드 위쪽으로 목표 회전치 생성.
        Quaternion targetRotation = Quaternion.LookRotation(forward.normalized, Vector3.up);

        // 현재 회전에서 목표 회전에 대해 보간 진행
        Quaternion nextRotation = Quaternion.Slerp(rb.rotation,targetRotation,recoverySpeed * Time.fixedDeltaTime);

        // 회전 적용
        rb.MoveRotation(nextRotation);
    }
}

// 뒤집힘 여부 판정
// 일정 시간 이상 뒤집혔을 때 복구
// 한번만 위로 튕기고 회전 적용
// 바라보는 방향 유지하면서 위쪽으로 보정
