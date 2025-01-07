using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
public partial struct InitializeEnemySystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        foreach (var (physicsMass, physicsVelocity, newEnemyEntity) in SystemAPI.Query<RefRW<PhysicsMass>, RefRW<PhysicsVelocity>>().WithAny<NewEnemyTag>().WithEntityAccess())
        {
            // •¨—‰‰Z‚É‚æ‚é‰ñ“]‚Ì–³Œø‰»F[0], [1], [2] ‚Í‚»‚ê‚¼‚ê X, Y, Z ²
            physicsMass.ValueRW.InverseInertia[0] = 0;
            physicsMass.ValueRW.InverseInertia[1] = 0;
            physicsMass.ValueRW.InverseInertia[2] = 0;

            // ŒJ‚è•Ô‚³‚È‚¢‚æ‚¤‚ÉNewEnemyTag‚Ííœ
            ecb.RemoveComponent<NewEnemyTag>(newEnemyEntity);
        }
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}
