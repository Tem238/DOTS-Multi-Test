using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct BeginAbilitySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var networkTime = SystemAPI.GetSingleton<NetworkTime>();
        if (!networkTime.IsFirstTimeFullyPredictingTick) return;

        var currentTick = networkTime.ServerTick;
        var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        foreach (var (ability, abilityBeginBuffer) in SystemAPI.Query<AbilityAspect, DynamicBuffer<AbilityBeginBufferElement>>().WithAll<Simulate>())
        {
            if (abilityBeginBuffer.IsEmpty) continue;
            foreach (var abilityBegin in abilityBeginBuffer)
            {
                var abilityData = ability.GetAbility(abilityBegin.AbilityIndex);
                var entity = ecb.Instantiate(abilityData.AbilityPrefab);
                ecb.SetComponent(entity, LocalTransform.FromPosition(ability.AttackPosition));
                ecb.AddComponent(entity, new DestroyOnTimer
                {
                    Timer = abilityData.DestroyOnTimer,
                });
            }
            abilityBeginBuffer.Clear();
        }
    }
}
