using System;

[Serializable]
public class PingPacket
{
    public string Type = "Ping";
    public long ClientTimeMs;
}