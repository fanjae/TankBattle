using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ServerConnector : MonoBehaviour
{
    [SerializeField] private string serverIp = "127.0.0.1";
    [SerializeField] private int serverPort = 7777;

    // 서버에서 받은 탱크 상태를 적용할 때 사용할 목록
    [SerializeField] private NetworkTankView[] tankViews;
    public int PlayerId { get; private set; }

    private TcpClient client;
    private NetworkStream stream;

    private async void Start()
    {
        await ConnectAsync();
    }

    private async Task ConnectAsync() // 비동기 연결 함수
    {
        try
        {
            // TCP 클라이언트 생성 후 서버에서 비동기로 연결한다.
            client = new TcpClient();

            await client.ConnectAsync(serverIp, serverPort);
            stream = client.GetStream();

            Debug.Log("Server connected.");

            // 서버 접속 확인용으로 보내는 확인 패킷 받고, Player Id 처리.
            string json = await ReceivePacketAsync();
            WelcomePacket packet = JsonUtility.FromJson<WelcomePacket>(json);

            PlayerId = packet.PlayerId;

            Debug.Log($"My PlayerId: {PlayerId}");

            // 상태 패킷 수신
            _ = ReceiveLoopAsync();
        }
        catch (Exception e)
        {
            Debug.LogError($"Server connect failed: {e.Message}");
        }
    }

    private async Task<string> ReceivePacketAsync()
    {
        // 길이 정보 읽어서 int로 변환
        byte[] lengthBuffer = await ReadExactAsync(4);
        int bodyLength = BitConverter.ToInt32(lengthBuffer, 0);

        // 본문 정보 읽어서 JSON 문자열로 변환
        byte[] bodyBuffer = await ReadExactAsync(bodyLength);
        return Encoding.UTF8.GetString(bodyBuffer);
    }

    private async Task<byte[]> ReadExactAsync(int size)
    {
        // 지정한 크기 만큼의 데이터 담을 버퍼
        byte[] buffer = new byte[size];
        int offset = 0;

        // size 만큼 읽도록 반복
        while (offset < size)
        {
            int read = await stream.ReadAsync(buffer, offset, size - offset);

            if (read == 0)
            {
                throw new Exception("Server disconnected.");
            }

            offset += read;
        }

        return buffer;
    }

    public async void SendJson<T>(T packet)
    {
        if (stream == null) return;

        try
        {
            // 패킷 객체 JSON 문자열로 변환
            string json = JsonUtility.ToJson(packet);
            byte[] body = Encoding.UTF8.GetBytes(json);

            // 본문 길이를 4바이트로 전송
            byte[] lengthPrefix = BitConverter.GetBytes(body.Length);
            await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);

            // 실제 내가 보내는 JSON 본문 내용 전송
            await stream.WriteAsync(body, 0, body.Length);
        }
        catch (Exception e)
        {
            Debug.LogError($"Send failed: {e.Message}");
        }
    }

    private async Task ReceiveLoopAsync()
    {
        try
        {
            while (true)
            {
                // 패킷 수신
                string json = await ReceivePacketAsync();

                // 상태 관련 패킷인지 확인후 처리
                if (json.Contains("\"Type\":\"State\""))
                {
                    StatePacket packet = JsonUtility.FromJson<StatePacket>(json);

                    if (packet.Tanks == null)
                    {
                        Debug.LogWarning("StatePacket.Tanks is null");
                        continue;
                    }

                    ApplyStatePacket(packet);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Receive loop stopped: {e.Message}");
        }
    }

    private void ApplyStatePacket(StatePacket packet)
    {
        // 탱크 뷰에 각 탱크 상태를 적용
        foreach (TankState state in packet.Tanks)
        {
            foreach (NetworkTankView view in tankViews)
            {
                if (view.PlayerId == state.PlayerId)
                {
                    view.ApplyState(state);
                    break;
                }
            }
        }
    }

    private void OnDestroy()
    {
        stream?.Close();
        client?.Close();
    }
}