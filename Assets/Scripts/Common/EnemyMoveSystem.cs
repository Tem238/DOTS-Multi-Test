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
            // �^�[�Q�b�g�ւ̌���(�P�ʃx�N�g��)���Z�o
            float3 direction = math.normalize(targetPosition.position - transform.Position);
            // �ʒu���X�V
            transform.Position += direction * commonStatus.MoveSpeed * DeltaTime;
        }
    }
}
