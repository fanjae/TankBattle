using System;

[Serializable]
public class PongPacket
{
    public string Type = "Pong";
    public long ClientTimeMs;
}