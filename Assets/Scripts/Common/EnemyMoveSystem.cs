using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateBefore(typeof(PlayerMoveSystem))]
[BurstCompile]
public partial struct EnemyMoveSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyTag>();
        state.RequireForUpdate<TargetPosition>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        new EnemyMoveJob
        {
            DeltaTime = deltaTime
        }.ScheduleParallel();
    }

    [BurstCompile]
    public partial struct EnemyMoveJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(ref LocalTransform transform, in TargetPosition targetPosition, in CommonStatus commonStatus, in Simulate _)
        {
            // ターゲットへの向き(単位ベクトル)を算出
            float3 direction = math.normalize(targetPosition.position - transform.Position);
            // 位置を更新
            transform.Position += direction * commonStatus.MoveSpeed * DeltaTime;
        }
    }
}
