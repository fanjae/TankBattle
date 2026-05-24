using UnityEngine;

public class CannonBall_Client : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float pushForce = 6f;
    [SerializeField] private float upwardForce = 0.8f;
    [SerializeField] private float torqueForce = 2.5f;
    [SerializeField] private int crashDamage = 5;

    private Vector3 lastPosition;
    private Vector3 moveDirection;

    private void Start()
    {
        lastPosition = transform.position;
        moveDirection = transform.forward;

        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Vector3 delta = transform.position - lastPosition;

        if (delta.sqrMagnitude > 0.0001f)
        {
            moveDirection = delta.normalized;
        }

        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody targetRb;

        if (collision.gameObject.TryGetComponent<Rigidbody>(out targetRb))
        {
            // 포탄 진행 방향 기준으로 수평 밀림만 먼저 계산
            Vector3 horizontalDir = moveDirection;
            horizontalDir.y = 0f;

            if (horizontalDir.sqrMagnitude < 0.0001f)
            {
                horizontalDir = transform.forward;
                horizontalDir.y = 0f;
            }

            horizontalDir.Normalize();

            // 앞으로 밀기
            targetRb.AddForce(horizontalDir * pushForce, ForceMode.Impulse);

            // 너무 뜨지 않게 약한 위쪽 힘만 추가
            targetRb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);

            // 진행 방향 기준으로 살짝 기울어지는 토크
            Vector3 torqueDir = Vector3.Cross(Vector3.up, horizontalDir).normalized;
            targetRb.AddTorque(torqueDir * torqueForce, ForceMode.Impulse);

            TankHealth tankHealth = collision.gameObject.GetComponentInParent<TankHealth>();

            if (tankHealth != null)
            {
                tankHealth.TakeDamage(crashDamage);
            }
        }

        Destroy(gameObject);
    }
}