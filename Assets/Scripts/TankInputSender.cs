using UnityEngine;

public class TankInputSender : MonoBehaviour
{
    // 서버 연결 담당 컴포넌트
    [SerializeField] private ServerConnector serverConnector;

    private float sendInterval = 1f / 30f;
    private float sendTimer;

    private void Update()
    {
        // 서버 연결 객체가 없으면 전송 안하며, 서버에서 PlayerId 할당 때까지 기다림.
        if (serverConnector == null) return;
        if (serverConnector.PlayerId == 0) return;

        // 입력 패킷을 일정 주기마다 전송.
        sendTimer += Time.deltaTime;

        if (sendTimer < sendInterval) return;

        sendTimer = 0f;

        // 현재 키 입력 상태를 패킷으로 만들어서 서버에 전송
        InputPacket packet = MakeInputPacket();
        serverConnector.SendJson(packet);
    }

    private InputPacket MakeInputPacket() // Input Packet에 입력 상태를 넣음.
    {
        InputPacket packet = new InputPacket();

        if (Input.GetKey(KeyCode.W)) packet.Move += 1f;
        if (Input.GetKey(KeyCode.S)) packet.Move -= 1f;

        if (Input.GetKey(KeyCode.D)) packet.Turn += 1f;
        if (Input.GetKey(KeyCode.A)) packet.Turn -= 1f;

        if (Input.GetKey(KeyCode.RightArrow)) packet.Turret += 1f;
        if (Input.GetKey(KeyCode.LeftArrow)) packet.Turret -= 1f;

        if (Input.GetKey(KeyCode.UpArrow)) packet.Gun -= 1f;
        if (Input.GetKey(KeyCode.DownArrow)) packet.Gun += 1f;

        packet.Fire = Input.GetKey(KeyCode.Space);

        bool fire = Input.GetKeyDown(KeyCode.Space);

        if (fire)
        {
            Debug.Log("Fire input detected");
        }

        return packet;
    }
}