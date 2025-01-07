using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// クライアントがサーバー接続時にスポーン場所を指定する用コンポーネント
/// </summary>
public struct ClientConnectRequest : IComponentData
{
    public float3 SpawnPosition;
}
