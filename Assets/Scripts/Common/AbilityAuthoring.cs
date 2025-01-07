using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// 各能力スロットの能力と有効化状態を保持
/// </summary>
public struct AbilitySlotBufferElement : IBufferElementData
{
    public Entity AbilitySlotEntity;
}

/*[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct AbilitySlotBufferElement : IBufferElementData
{
    [GhostField] public AbilitySlot AbilitySlot;
}*/

[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct AbilitySlot : IComponentData
{
    public int SlotIndex;
    public int AbilityIndex;
    public uint CoolDownTick;
    public bool IsActive;
}

public struct CooldownTargetTick : IComponentData
{
    public NetworkTick Value;
}

public class AbilityAuthoring : MonoBehaviour
{
    public int AbilitySlotCount = 2;

    public class Baker : Baker<AbilityAuthoring>
    {
        public override void Bake(AbilityAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            var buffer = AddBuffer<AbilitySlotBufferElement>(entity);
            for (int i = 1; i <= authoring.AbilitySlotCount; i++)
            {
                var abilitySlotEntity = CreateAdditionalEntity(TransformUsageFlags.None, false, $"AbilitySlot{i}");

                AddComponent(abilitySlotEntity, new AbilitySlot
                {
                    SlotIndex = i,
                    AbilityIndex = 1,
                    CoolDownTick = 60u,
                    IsActive = false,
                });

                buffer.Add(new AbilitySlotBufferElement
                {
                    AbilitySlotEntity = abilitySlotEntity
                });
            }
        }
    }
}
