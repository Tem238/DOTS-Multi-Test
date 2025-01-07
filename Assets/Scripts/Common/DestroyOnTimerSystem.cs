using Unity.Burst;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
[BurstCompile]
public partial struct DestroyOnTimerSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);

        var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;

        foreach(var (destroyAtTick, entity) in SystemAPI.Query<DestroyAtTick>().WithAll<Simulate>().WithNone<DestroyEntityTag>().WithEntityAccess())
        {
            if(currentTick.Equals(destroyAtTick.Tick) || currentTick.IsNewerThan(destroyAtTick.Tick))
            {
                ecb.AddComponent<DestroyEntityTag>(entity);
            }
        }
    }
}
