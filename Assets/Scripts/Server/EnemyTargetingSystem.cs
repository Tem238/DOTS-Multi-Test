using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct EnemyTargetingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<TargetPosition>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // ジョブに渡すプレイヤー位置のリストを作成
        var playerPositions = new NativeList<float3>(Allocator.TempJob);

        // PlayerTag を持つエンティティの位置を取得
        foreach (var transform in SystemAPI.Query<LocalTransform>().WithAll<PlayerTag>())
        {
            playerPositions.Add(transform.Position);
        }
        
        if (playerPositions.Length == 0)
        {
            return;
        }

        // TargetPosition を更新するジョブをスケジュール
        var job = new UpdateTarget
        {
            PlayerPositions = playerPositions
        };
        job.ScheduleParallel(state.Dependency).Complete();

        playerPositions.Dispose();
    }

    [BurstCompile]
    public partial struct UpdateTarget : IJobEntity
    {
        // マルチスレッドで実行するため、NativeListはReadOnly指定
        [ReadOnly] public NativeList<float3> PlayerPositions;

        private void Execute(ref TargetPosition targetPosition, in LocalTransform transform)
        {
            float3 closestPosition = transform.Position;
            float closestSqrDistance = float.MaxValue;

            // 各プレイヤーとエネミー間の距離を算出し、一番近いプレイヤーの位置を探す
            foreach (var playerPosition in PlayerPositions)
            {
                var sqrDistance = math.distancesq(transform.Position, playerPosition);
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestPosition = playerPosition;
                }
            }
            // 一番近いプレイヤーの位置をセット
            targetPosition.position = closestPosition;
        }
    }
}
