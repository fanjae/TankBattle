using System;
using UnityEngine;

[Serializable] // JsonUtility로 변홯나기 위한 입력 패킷
public class InputPacket
{
    public string Type = "Input";
    public float Move;
    public float Turn;
    public float Turret;
    public float Gun;
    public bool Fire;
}
