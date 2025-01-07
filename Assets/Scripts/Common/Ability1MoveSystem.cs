using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[UpdateAfter(typeof(BeginAbilitySystem))]
[BurstCompile]
public partial struct AbilityMoveSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;

        new AbilityMoveJob
        {
            DeltaTime = deltaTime
        }.Schedule();
    }

    public partial struct AbilityMoveJob : IJobEntity
    {
        public float DeltaTime;

        private void Execute(ref LocalTransform transform, in Ability1Status ability1, in Simulate _)
        {
            transform.Position += ability1.Forward * ability1.MoveSpeed * DeltaTime;
        }
    }
}
