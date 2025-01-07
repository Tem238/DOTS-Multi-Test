using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(PhysicsSimulationGroup))]
public partial struct DamageOnTriggerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var damageOnTriggerJob = new DamageOnTriggerJob
        {
            DamageOnTriggerLookup = SystemAPI.GetComponentLookup<DamageOnTrigger>(true),
            //TeamLookup = SystemAPI.GetComponentLookup<MobaTeam>(true),
            AlreadyDamagedLookup = SystemAPI.GetBufferLookup<AlreadyDamagedEntity>(true),
            DamageBufferLookup = SystemAPI.GetBufferLookup<DamageBufferElement>(true),
            ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged)
        };
        var simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
        state.Dependency = damageOnTriggerJob.Schedule(simulationSingleton, state.Dependency);
    }
}

public struct DamageOnTriggerJob : ITriggerEventsJob
{
    [ReadOnly] public ComponentLookup<DamageOnTrigger> DamageOnTriggerLookup;
    // [ReadOnly] public ComponentLookup<MobaTeam> TeamLookup;
    [ReadOnly] public BufferLookup<AlreadyDamagedEntity> AlreadyDamagedLookup;
    [ReadOnly] public BufferLookup<DamageBufferElement> DamageBufferLookup;

    public EntityCommandBuffer ECB;

    public void Execute(TriggerEvent triggerEvent)
    {
        // �_���[�W��^����G���e�B�e�B�ƃ_���[�W���󂯂�G���e�B�e�B���擾
        Entity damageDealingEntity;
        Entity damageReceivingEntity;

        if (DamageBufferLookup.HasBuffer(triggerEvent.EntityA) && 
            DamageOnTriggerLookup.HasComponent(triggerEvent.EntityB))
        {
            damageReceivingEntity = triggerEvent.EntityA;
            damageDealingEntity = triggerEvent.EntityB;
        }
        else if (DamageOnTriggerLookup.HasComponent(triggerEvent.EntityA) && 
                 DamageBufferLookup.HasBuffer(triggerEvent.EntityB))
        {
            damageDealingEntity = triggerEvent.EntityA;
            damageReceivingEntity = triggerEvent.EntityB;
        }
        else
        {
            return;
        }

        // �����U�����d�����ăq�b�g���Ȃ��悤�ɂ���
        var alreadyDamagedBuffer = AlreadyDamagedLookup[damageDealingEntity];
        foreach(var alreadyDamagedEntity in alreadyDamagedBuffer)
        {
            // ���Ƀ_���[�W��^�����G���e�B�e�B�Ȃ�I��
            if (alreadyDamagedEntity.entity.Equals(damageReceivingEntity)) return;
        }

        /*// �t�����h���[�t�@�C�A���Ȃ��悤�ɂ���
        if(TeamLookup.TryGetComponent(damageDealingEntity, out var damageDealingTeam) &&
            TeamLookup.TryGetComponent(damageReceiveEntity, out var damageReceivingTeam))
        {
            if (damageDealingTeam.Value == damageReceivingTeam.Value) return;
        }*/

        // �_���[�W��^����E�d���_���[�W��h���R���|�[�l���g�����ꂼ��ǉ�
        var damageOnTrigger = DamageOnTriggerLookup[damageDealingEntity];
        ECB.AppendToBuffer(damageReceivingEntity, new DamageBufferElement { Damage = damageOnTrigger.Damage });
        ECB.AppendToBuffer(damageDealingEntity, new AlreadyDamagedEntity { entity = damageReceivingEntity });
    }
}
