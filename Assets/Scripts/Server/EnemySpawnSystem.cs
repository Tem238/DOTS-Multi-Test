using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    private Random _random;
    private NetworkTick _startTick;
    private int _simulationTickRate;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyPrefabs>();
        state.RequireForUpdate<NetworkTime>();
        // とりあえずシード値は100固定でランダム生成
        _random = new Random(100);
        // NetCodeConfigからレートを取得
        _simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var networkTime = SystemAPI.GetSingleton<NetworkTime>();

        // サーバー開始時にTickをフィールドに保持
        if (!_startTick.IsValid)
        {
            _startTick = networkTime.ServerTick;
            return;
        }

        // サーバー開始時と現在のTickの差
        var elapsedTick = networkTime.ServerTick.TicksSince(_startTick);
        // 1秒に1回敵を生成する、それ以外は早期リターン
        if(elapsedTick % _simulationTickRate != 0) return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        // 生成する敵のエンティティを取得して生成
        var enemy1Entity = SystemAPI.GetSingleton<EnemyPrefabs>().Enemy1;
        var spawnEntity = ecb.Instantiate(enemy1Entity);
        
        ecb.SetName(spawnEntity, "Enemy1");
        var spawnPosition = new float3(_random.NextFloat(-10f, 10f), _random.NextFloat(-10f, 10f), 0f);
        var newTransform = LocalTransform.FromPosition(spawnPosition);
        ecb.SetComponent(spawnEntity, newTransform);

        ecb.Playback(state.EntityManager);
        // EntityCommandBuffer(Allocator.Temp)はメモリリークする可能性があるため最後に解放
        ecb.Dispose();
    }
}
