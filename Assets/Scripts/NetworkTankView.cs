using UnityEngine;

public class NetworkTankView : MonoBehaviour
{
    [SerializeField] private int playerId;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform gun;

    private Quaternion initialGunLocalRotation;
    public int PlayerId => playerId;

    private void Awake()
    {
        if (gun != null)
            initialGunLocalRotation = gun.localRotation;
    }

    // 서버에서 받은 탱크 상태를 Unity 오브젝트에 반영
    public void ApplyState(TankState state)
    {
        transform.position = new Vector3(state.X, transform.position.y, state.Z); // 탱크 위치 상태 반영
        transform.rotation = Quaternion.Euler(0f, state.Angle, 0f); // 탱크 회전 상태 반영

        if (turret != null) // 터렛의 회전 상태 반영
            turret.localRotation = Quaternion.Euler(0f, state.TurretTurn, 0f);

        if (gun != null) // 포신 각도 반영
            gun.localRotation = initialGunLocalRotation * Quaternion.Euler(state.GunPitch, 0f, 0f);
    }
}