using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

// サーバーでのみ実行
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerProcessGameEntryRequestSystem : ISystem
{
    // サーバーへの通信(RPCリクエスト)がある場合のみ実行
    public void OnCreate(ref SystemState state)
    {
        // クライアントが送信したSendRpcCommandRequestをサーバーがReceiveRpcCommandRequestで受け取る
        var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<ConnectRequest, ReceiveRpcCommandRequest>();
        state.RequireForUpdate(state.GetEntityQuery(builder));
        state.RequireForUpdate<PlayerPrefabs>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var playerPrefab = SystemAPI.GetSingleton<PlayerPrefabs>().Player;

        foreach(var (connectRequest, requestSource, requestEntity) in 
            SystemAPI.Query<ConnectRequest, ReceiveRpcCommandRequest>().WithEntityAccess())
        {
            // RPCリクエストは1回だけ処理したいので、そのリクエストのエンティティは削除する
            ecb.DestroyEntity(requestEntity);
            ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

            // クライアントのネットワークIDを取得
            var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
            Debug.Log($"サーバーに接続しました。 ID:{clientId}");

            // プレイヤーの生成
            var newPlayer = ecb.Instantiate(playerPrefab);
            // わかりやすいようにオブジェクトの名前を付けておきます
            ecb.SetName(newPlayer, "Player");
            // リクエストされた場所に移動
            var newTransform = LocalTransform.FromPosition(connectRequest.SpawnPositioin);
            ecb.SetComponent(newPlayer, newTransform);
            // ゴースト（クライアントに表示される方オブジェクト）の所有者を生成をリクエストしたクライアントに指定
            ecb.SetComponent(newPlayer, new GhostOwner { NetworkId = clientId });

            ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = newPlayer });
        }
        ecb.Playback(state.EntityManager);
    }
}
