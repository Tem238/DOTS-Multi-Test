using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct EnemyTargetingSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerTag>();
        state.RequireForUpdate<TargetPosition>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        // �W���u�ɓn���v���C���[�ʒu�̃��X�g���쐬
        var playerPositions = new NativeList<float3>(Allocator.TempJob);

        // PlayerTag �����G���e�B�e�B�̈ʒu���擾
        foreach (var transform in SystemAPI.Query<LocalTransform>().WithAll<PlayerTag>())
        {
            playerPositions.Add(transform.Position);
        }
        
        if (playerPositions.Length == 0)
        {
            return;
        }

        // TargetPosition ���X�V����W���u���X�P�W���[��
        var job = new UpdateTarget
        {
            PlayerPositions = playerPositions
        };
        job.ScheduleParallel(state.Dependency).Complete();

        playerPositions.Dispose();
    }

    [BurstCompile]
    public partial struct UpdateTarget : IJobEntity
    {
        // �}���`�X���b�h�Ŏ��s���邽�߁ANativeList��ReadOnly�w��
        [ReadOnly] public NativeList<float3> PlayerPositions;

        private void Execute(ref TargetPosition targetPosition, in LocalTransform transform)
        {
            float3 closestPosition = transform.Position;
            float closestSqrDistance = float.MaxValue;

            // �e�v���C���[�ƃG�l�~�[�Ԃ̋������Z�o���A��ԋ߂��v���C���[�̈ʒu��T��
            foreach (var playerPosition in PlayerPositions)
            {
                var sqrDistance = math.distancesq(transform.Position, playerPosition);
                if (sqrDistance < closestSqrDistance)
                {
                    closestSqrDistance = sqrDistance;
                    closestPosition = playerPosition;
                }
            }
            // ��ԋ߂��v���C���[�̈ʒu���Z�b�g
            targetPosition.position = closestPosition;
        }
    }
}
