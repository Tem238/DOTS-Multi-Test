using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

// サーバー・クライアント両方で実行される
[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct PlayerMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        new PlayerMoveJob
        {
            DeltaTime = deltaTime
        }.Schedule();
    }

    [BurstCompile]
    public partial struct PlayerMoveJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(ref LocalTransform transform, in PlayerInput playerInput, in CommonStatus playerStatus, in Simulate simulate)
        {
            transform.Position.xy += playerInput.MoveValue * playerStatus.MoveSpeed * DeltaTime;
        }
    }
}
