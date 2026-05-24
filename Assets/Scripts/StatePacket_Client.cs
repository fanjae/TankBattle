using System;

[Serializable] // 탱크 상태를 처리하기 위한 용도의 패킷
public class StatePacket 
{
    public string Type = "State";
    public TankState[] Tanks;
    public CannonBallState[] CannonBalls;
}