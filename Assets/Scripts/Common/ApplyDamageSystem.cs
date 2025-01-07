using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderLast = true)]
[UpdateAfter(typeof(CalculateFrameDamageSystem))]
public partial struct ApplyDamageSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkTime>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var (commonStatus, damageThisTickBuffer, entity) in SystemAPI.Query<RefRW<CommonStatus>, DynamicBuffer<DamageThisTick>>().WithAll<Simulate>().WithEntityAccess())
        {
            if (!damageThisTickBuffer.GetDataAtTick(currentTick, out var damageThisTick)) continue;
            if (damageThisTick.Tick != currentTick) continue;
            commonStatus.ValueRW.HP -= damageThisTick.Damage;

            if(commonStatus.ValueRO.HP <= 0)
            {
                ecb.AddComponent<DestroyEntityTag>(entity);
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
