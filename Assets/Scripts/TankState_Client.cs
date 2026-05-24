using System;

[Serializable] // 탱크 상태 정보
public class TankState
{
    public int PlayerId;

    public float X;
    public float Z;
    public float Angle;

    public float TurretTurn;
    public float GunPitch;

    public int Hp;
}