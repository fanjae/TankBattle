using UnityEngine;

public class TankMove : MonoBehaviour
{
    [SerializeField]
    private float tankSpeed = 5f;
    [SerializeField]
    private float reverseSpeed = 3f;
    [SerializeField]
    private float turnSpeed = 120f;

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out rb) == false)
        {
            Debug.LogError("Rigidbody Not Found");
        }

    }
    private void Update()
    {
        moveInput = 0f;
        turnInput = 0f;

        if (Input.GetKey(KeyCode.W)) moveInput += 1f;
        if (Input.GetKey(KeyCode.S)) moveInput -= 1f;

        if (Input.GetKey(KeyCode.D)) turnInput += 1f;
        if (Input.GetKey(KeyCode.A)) turnInput -= 1f;
    }
    private void FixedUpdate()
    {
        Move();
        Turn();
    }
    private void Move()
    {
        float speed = moveInput >= 0 ? tankSpeed : reverseSpeed; // 전진 속도와 후진 속도 처리 
        Vector3 move = transform.forward * moveInput * speed * Time.fixedDeltaTime; 
        
        rb.MovePosition(rb.position + move); // Rigidbody 이동
    }

    private void Turn()
    {
        float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion rotation = Quaternion.Euler(0, turn, 0); // Y축 기준으로 회전.


        rb.MoveRotation(rb.rotation * rotation); // Rigibody 회전

    }    
}
