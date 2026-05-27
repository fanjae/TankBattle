using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerConnector : MonoBehaviour
{
    [SerializeField] private string serverIp; [SerializeField] private int serverPort = 7777;

    // 연결 후 카메라랑 발사표시 처리
    [SerializeField] private Camera mainCamera;
    [SerializeField] private AimFollow aimFollow;

    // 서버에서 받은 탱크 상태를 적용할 때 사용할 목록
    [SerializeField] private NetworkTankView[] tankViews;
    [SerializeField] private GameObject cannonBallPrefab;

    [SerializeField] private TMP_Text p1HpText;
    [SerializeField] private TMP_Text p2HpText;

    [SerializeField] private TMP_Text pingText;
    private float pingTimer;

    private bool isConnected;
    private const int PingIntervalMs = 1000;

    public int PlayerId { get; private set; }

    private TcpClient client;
    private NetworkStream stream;

    private const int MaxPacketSize = 4096;

    private readonly Dictionary<int, GameObject> cannonBallObjects = new();
    private readonly SemaphoreSlim sendLock = new SemaphoreSlim(1, 1); // 송신 락


    private async void Start()
    {
        serverIp = PlayerPrefs.GetString("ServerIP", "127.0.0.1");
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

            // 카메라 설정
            AttachCameraToMyTank();

            isConnected = true;

            // 상태 패킷 수신
            _ = ReceiveLoopAsync();
            _ = PingLoopAsync();

        }
        catch (Exception e)
        {
            Debug.LogError($"Server connect failed: {e.Message}");

            PlayerPrefs.SetString("MenuMessage", "Unable to connect to the server.");
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    private async Task<string> ReceivePacketAsync()
    {
        // 길이 정보 읽어서 int로 변환
        byte[] lengthBuffer = await ReadExactAsync(4);
        int bodyLength = BitConverter.ToInt32(lengthBuffer, 0);

        if (bodyLength <= 0 || bodyLength > MaxPacketSize)
        {
            throw new Exception($"Invalid packet size: {bodyLength}");
        }

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

        await sendLock.WaitAsync();

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
        finally
        {
            sendLock.Release();
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
                else if (json.Contains("\"Type\":\"Pong\""))
                {
                    PongPacket packet = JsonUtility.FromJson<PongPacket>(json);

                    long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    long rtt = now - packet.ClientTimeMs;

                    Debug.Log($"Ping: {rtt} ms");

                    if (pingText != null)
                    {
                        pingText.text = $"PING : {rtt} ms";
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Receive loop stopped: {e.Message}");
        }
    }

    private async Task PingLoopAsync()
    {
        while (isConnected && stream != null)
        {
            PingPacket packet = new PingPacket
            {
                ClientTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            SendJson(packet);

            await Task.Delay(PingIntervalMs);
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

            UpdateHpUI(state); // HP 상태 변경
        }

        if (packet.CannonBalls != null)
        {
            ApplyCannonBallStates(packet.CannonBalls);
        }
    }

    private void OnDestroy() // 네트워크 리소스 정리
    {
        isConnected = false;

        foreach (GameObject cannonBall in cannonBallObjects.Values)
        {
            Destroy(cannonBall);
        }

        cannonBallObjects.Clear();

        stream?.Close();
        client?.Close();
    }

    // 서버에서 받은 포탄 상태를 기준으로 클라이언트에 생성, 위치 갱신, 제거
    private void ApplyCannonBallStates(CannonBallState[] states)
    {
        HashSet<int> aliveIds = new();

        foreach (CannonBallState state in states)
        {
            int id = state.CannonBallId;
            aliveIds.Add(id);

            // 서버에서 계산한 포탄 위치
            Vector3 position = new Vector3(state.X, state.Y, state.Z);

            // 생성 안된 포탄 새로 생성
            if (cannonBallObjects.TryGetValue(id, out GameObject cannonBall) == false ||
                cannonBall == null)
            {
                cannonBall = Instantiate(cannonBallPrefab, position, Quaternion.identity);
                cannonBallObjects[id] = cannonBall;

                // 발사자 탱크와의 충돌 무시
                IgnoreOwnerCollision(cannonBall, state.OwnerPlayerId);
            }

            // 기존 포탄 위치 서버랑 동기화
            cannonBall.transform.position = position;
        }

        // 없는 포탄 제거
        RemoveMissingCannonBalls(aliveIds);
    }

    // 포탄이 생성 직후 자신의 탱크와 충돌하지 않게 처리
    private void IgnoreOwnerCollision(GameObject cannonBall, int ownerPlayerId)
    {
        Collider ballCollider = cannonBall.GetComponent<Collider>();
        if (ballCollider == null) return;

        foreach (NetworkTankView view in tankViews)
        {
            if (view.PlayerId != ownerPlayerId) continue;

            Collider tankCollider = view.GetComponent<Collider>();

            if (tankCollider != null)
                Physics.IgnoreCollision(ballCollider, tankCollider, true);

            return;
        }
    }

    // 서버 상태 패킷에 없는 포탄은 제거
    private void RemoveMissingCannonBalls(HashSet<int> aliveIds)
    {
        // Key 목록을 복사해서 순회한다.
        // Dictionary는 순회 중 수정이 불가함.
        foreach (int id in new List<int>(cannonBallObjects.Keys))
        {
            if (aliveIds.Contains(id)) continue;

            if (cannonBallObjects[id] != null)
                Destroy(cannonBallObjects[id]);

            cannonBallObjects.Remove(id);
        }
    }

    // 서버에서 받은 PlayerID를 기준으로 내 탱크를 찾게 만들기
    private void AttachCameraToMyTank()
    {
        foreach (NetworkTankView view in tankViews)
        {
            if (view.PlayerId != PlayerId) continue;

            // 카메라 지점과 포신 끝 위치를 찾음
            Transform cameraPoint = view.transform.Find("Turret/CameraPoint");
            Transform gunAimPoint = view.transform.Find("Turret/Gun/GunFireStartingPoint");

            if (cameraPoint == null)
            {
                Debug.LogError("CameraPoint not found.");
                return;
            }

            // 메인 카메라를 탱크 카메라 Point에 붙이고, 위치랑 회전상태 초기화 및 십자 표시(조준점)이 탱크 포신 위치 따라가게 설정
            mainCamera.transform.SetParent(cameraPoint);
            mainCamera.transform.localPosition = Vector3.zero;
            mainCamera.transform.localRotation = Quaternion.identity;

            aimFollow.SetTarget(mainCamera, gunAimPoint);

            return;
        }
    }

    // HP 상태 변경
    private void UpdateHpUI(TankState state)
    {
        if (state.PlayerId == 1 && p1HpText != null)
        {
            p1HpText.text = $"P1 HP : {state.Hp}";
        }
        else if (state.PlayerId == 2 && p2HpText != null)
        {
            p2HpText.text = $"P2 HP : {state.Hp}";
        }
    }
}