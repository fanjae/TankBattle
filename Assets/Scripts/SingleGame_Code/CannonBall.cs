using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float pushForce = 10f;
    [SerializeField] private float upwardForce = 1f;
    [SerializeField] private int crashDamage = 5;

    private TankHealth ownerTankHealth;

    public void SetOwner(TankHealth owner)
    {
        ownerTankHealth = owner;
    }
    private void Start()
    {
        Destroy(gameObject, lifeTime); // 3초 뒤 삭제
    }

    private void OnCollisionEnter(Collision collision)
    {
        TankHealth hitTankHealth = collision.gameObject.GetComponentInParent<TankHealth>();

        // 내가 쏜 포탄이 내 탱크에 맞은 경우 무시
        if (hitTankHealth != null && hitTankHealth == ownerTankHealth)
        {
            return;
        }

        Rigidbody targetRb;

        if (collision.gameObject.TryGetComponent<Rigidbody>(out targetRb))
        {
            Vector3 pushDir = transform.forward;
            pushDir.y += upwardForce;
            pushDir.Normalize();

            targetRb.AddForce(pushDir * pushForce, ForceMode.Impulse);

            // 충돌한 오브젝트 부모쪽에서 TankHealth 컴포넌트 찾기
            TankHealth tankHealth = collision.gameObject.GetComponentInParent<TankHealth>();

            // 체력 컴포넌트 존재시 충돌 데미지 적용
            if (tankHealth != null)
            {
                tankHealth.TakeDamage(crashDamage);
            }
        }

        Destroy(gameObject);
    }
}
