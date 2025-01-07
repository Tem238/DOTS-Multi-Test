using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

// SimulationSystemGroup�����̒��ōŏ��ɍs�������Ƃ���
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct InitializePlayerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach(var (physicsMass, physicsVelocity, newPlayerEntity) in SystemAPI.Query<RefRW<PhysicsMass>, RefRW<PhysicsVelocity>>().WithAny<NewPlayerTag>().WithEntityAccess())
        {
            // �������Z�ɂ���]�̖������F[0], [1], [2] �͂��ꂼ�� X, Y, Z ��
            physicsMass.ValueRW.InverseInertia[0] = 0;
            physicsMass.ValueRW.InverseInertia[1] = 0;
            physicsMass.ValueRW.InverseInertia[2] = 0;

            // �킩��₷���悤�ɐF��ԂɕύX
            ecb.SetComponent(newPlayerEntity, new URPMaterialPropertyBaseColor { Value = new float4(1, 0, 0, 1) });
            // �J��Ԃ��Ȃ��悤��NewPlayerTag�͍폜
            ecb.RemoveComponent<NewPlayerTag>(newPlayerEntity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
