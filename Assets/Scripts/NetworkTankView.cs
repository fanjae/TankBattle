using UnityEngine;

public class NetworkTankView : MonoBehaviour
{
    [SerializeField] private int playerId;
    [SerializeField] private Transform turret;
    [SerializeField] private Transform gun;
    [SerializeField] private TankHpBar hpBar;

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
        transform.position = new Vector3(state.X, transform.position.y, state.Z);

        transform.rotation = Quaternion.Euler(0f, state.Angle, 0f);

        if (turret != null)
            turret.localRotation = Quaternion.Euler(0f, state.TurretTurn, 0f);

        if (gun != null)
            gun.localRotation = initialGunLocalRotation * Quaternion.Euler(-state.GunPitch, 0f, 0f);

        if (hpBar != null)
            hpBar.SetHp(state.Hp);
    }
}