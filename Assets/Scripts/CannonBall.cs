using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] private float lifeTime = 3f;
    private void Start()
    {
        Destroy(gameObject, lifeTime); // 3초 뒤 삭제
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Hit : {collision.gameObject.name}"); // 충돌한 콜리더 오브젝트 이름 출력

        Destroy(gameObject); // 삭제
    }
}
