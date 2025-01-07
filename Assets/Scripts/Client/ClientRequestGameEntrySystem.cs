using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;

// クライアントおよびシンクライアント（テストのためのNPCみたいな）のみで実行
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientRequestGameEntrySystem : ISystem
{
    // まだサーバーに接続していないネットワークIDを持つコンポーネントを探すクエリ
    private EntityQuery _pendingNetworkIdQuery;

    public void OnCreate(ref SystemState state)
    {
        // 接続待機中のエンティティがある時だけOnUpdateの処理を実行
        var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
        _pendingNetworkIdQuery = state.GetEntityQuery(builder);
        state.RequireForUpdate(_pendingNetworkIdQuery);
        state.RequireForUpdate<ClientConnectRequest>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // リクエストされたスポーン場所を取得
        var requestedSpawnPosition = SystemAPI.GetSingleton<ClientConnectRequest>().SpawnPosition;
        // エンティティを操作（コンポーネントの追加・削除・編集など）するためのバッファー
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        // 接続待機中のエンティティをすべて取得
        var pendingNetworkIds = _pendingNetworkIdQuery.ToEntityArray(Allocator.Temp);

        foreach(var pendingNetworkId in pendingNetworkIds)
        {
            // NetworkStreamInGameコンポーネントを待機中のエンティティに追加して、接続状態にする
            ecb.AddComponent<NetworkStreamInGame>(pendingNetworkId);

            // サーバーへのリクエスト用エンティティを作成
            var requestConnectEntity = ecb.CreateEntity();
            // RPCコマンドに取得したスポーン場所を込めてサーバーへ送信
            ecb.AddComponent(requestConnectEntity, new ConnectRequest { SpawnPositioin = requestedSpawnPosition });
            ecb.AddComponent(requestConnectEntity, new SendRpcCommandRequest { TargetConnection = pendingNetworkId });
        }
        ecb.Playback(state.EntityManager);
    }
}
