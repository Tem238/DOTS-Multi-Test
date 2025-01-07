using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct AbilitySlotManageSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkTime>();
        state.RequireForUpdate<AbilitySlot>();
        state.RequireForUpdate<CooldownTargetTick>();
        state.RequireForUpdate<OwnerPlayerTag>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var currentTick = SystemAPI.GetSingleton<NetworkTime>().ServerTick;
        var ownerPlayerEntity = SystemAPI.GetSingletonEntity<OwnerPlayerTag>();

        foreach (var (abilitySlot, cooldownTargetTick, entity) in SystemAPI.Query<RefRO<AbilitySlot>, RefRO<CooldownTargetTick>>().WithEntityAccess())
        {
            // スロットがアクティブでなければ処理終了
            if (!abilitySlot.ValueRO.IsActive || !currentTick.IsNewerThan(cooldownTargetTick.ValueRO.Value))
            {
                continue;
            }

            ecb.RemoveComponent<CooldownTargetTick>(entity);
            ecb.AppendToBuffer(ownerPlayerEntity, new AbilityBeginBufferElement
            {
                AbilityIndex = abilitySlot.ValueRO.AbilityIndex,
            });
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
