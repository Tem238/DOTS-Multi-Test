using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Rendering;

// SimulationSystemGroup処理の中で最初に行う処理とする
[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct InitializePlayerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach(var (physicsMass, physicsVelocity, newPlayerEntity) in SystemAPI.Query<RefRW<PhysicsMass>, RefRW<PhysicsVelocity>>().WithAny<NewPlayerTag>().WithEntityAccess())
        {
            // 物理演算による回転の無効化：[0], [1], [2] はそれぞれ X, Y, Z 軸
            physicsMass.ValueRW.InverseInertia[0] = 0;
            physicsMass.ValueRW.InverseInertia[1] = 0;
            physicsMass.ValueRW.InverseInertia[2] = 0;

            // わかりやすいように色を赤に変更
            ecb.SetComponent(newPlayerEntity, new URPMaterialPropertyBaseColor { Value = new float4(1, 0, 0, 1) });
            // 繰り返さないようにNewPlayerTagは削除
            ecb.RemoveComponent<NewPlayerTag>(newPlayerEntity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
