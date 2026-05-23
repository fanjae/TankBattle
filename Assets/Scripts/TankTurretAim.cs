using UnityEngine;

public class TankTurretAim : MonoBehaviour
{
    [SerializeField]
    private float turretTurnSpeed = 120f;
    private void Update()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.RightArrow)) input += 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) input -= 1f;

        // 프레임 단위 회전 각도 계산.
        float turn = input * turretTurnSpeed * Time.deltaTime;

        // 로컬 좌표 기준 회전
        transform.Rotate(0f, turn, 0f, Space.Self); 
    }
}
