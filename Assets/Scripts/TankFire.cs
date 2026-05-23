using UnityEngine;

public class TankFire : MonoBehaviour
{
    [SerializeField] 
    private Transform firePoint; // 발사 위치
    [SerializeField] 
    private GameObject cannonBallPrefab; // 포탄 Prefab

    [SerializeField]
    private float fireForce = 5f;

    [SerializeField]
    private float coolTime = 0.5f;

    private float lastFireTime; // 마지막 발사 시간

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스 버튼 누르면 대포 발사
        {
            Fire();
        }
    }
    private void Fire()
    {
        // 쿨타임 계산
        if (Time.time - lastFireTime < coolTime) return ;

        // 시간 갱신
        lastFireTime = Time.time;

        // 발사 위치와 회전값으로 포탄 Prefab 생성
        GameObject cannonBall = Instantiate(cannonBallPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb;
        if (TryGetComponent<Rigidbody>(out rb) == false) // 포탄 Component 획득
        {
            Debug.LogError("Rigidbody Not Found.");
            return ;
        }

        rb.AddForce(firePoint.forward * fireForce, ForceMode.Impulse);
        // 포신 바라보는 방향으로, 포탄 발사
    }
}
