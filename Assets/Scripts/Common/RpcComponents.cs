using Unity.Mathematics;
using Unity.NetCode;

public struct ConnectRequest : IRpcCommand
{
    public float3 SpawnPositioin;
}
