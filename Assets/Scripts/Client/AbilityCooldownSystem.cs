using Unity.Entities;
using Unity.NetCode;
using Unity.Collections;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct AbilityCooldownSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<AbilitySlot>();
        state.RequireForUpdate<NetworkTime>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach(var (abilitySlot, entity) in SystemAPI.Query<RefRO<AbilitySlot>>().WithNone<CooldownTargetTick>().WithEntityAccess())
        {
            var coolDownTargetTick = currentTick;
            coolDownTargetTick.Add(abilitySlot.ValueRO.CoolDownTick);
            ecb.AddComponent(entity, new CooldownTargetTick
            {
                Value = coolDownTargetTick,
            });
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
