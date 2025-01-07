using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
public partial struct AbilitySlotActivateSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        foreach (var ability in SystemAPI.Query<AbilityAspect>().WithAll<Simulate>())
        {
            foreach(var slot in ability.AbilitySlots)
            {
                var slotEntity = slot.AbilitySlotEntity;
                var abilitySlot = SystemAPI.GetComponent<AbilitySlot>(slotEntity);
                if (ability.GetToggle(abilitySlot.SlotIndex))
                {
                    var updateSlot = abilitySlot;
                    updateSlot.IsActive = !abilitySlot.IsActive;
                    ecb.SetComponent(slotEntity, updateSlot);
                }
            }
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
