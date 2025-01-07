using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
[BurstCompile]
public partial struct EnemySpawnSystem : ISystem
{
    private Random _random;
    private NetworkTick _startTick;
    private int _simulationTickRate;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<EnemyPrefabs>();
        state.RequireForUpdate<NetworkTime>();
        // �Ƃ肠�����V�[�h�l��100�Œ�Ń����_������
        _random = new Random(100);
        // NetCodeConfig���烌�[�g���擾
        _simulationTickRate = NetCodeConfig.Global.ClientServerTickRate.SimulationTickRate;
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var networkTime = SystemAPI.GetSingleton<NetworkTime>();

        // �T�[�o�[�J�n����Tick���t�B�[���h�ɕێ�
        if (!_startTick.IsValid)
        {
            _startTick = networkTime.ServerTick;
            return;
        }

        // �T�[�o�[�J�n���ƌ��݂�Tick�̍�
        var elapsedTick = networkTime.ServerTick.TicksSince(_startTick);
        // 1�b��1��G�𐶐�����A����ȊO�͑������^�[��
        if(elapsedTick % _simulationTickRate != 0) return;

        var ecb = new EntityCommandBuffer(Allocator.Temp);
        // ��������G�̃G���e�B�e�B���擾���Đ���
        var enemy1Entity = SystemAPI.GetSingleton<EnemyPrefabs>().Enemy1;
        var spawnEntity = ecb.Instantiate(enemy1Entity);
        
        ecb.SetName(spawnEntity, "Enemy1");
        var spawnPosition = new float3(_random.NextFloat(-10f, 10f), _random.NextFloat(-10f, 10f), 0f);
        var newTransform = LocalTransform.FromPosition(spawnPosition);
        ecb.SetComponent(spawnEntity, newTransform);

        ecb.Playback(state.EntityManager);
        // EntityCommandBuffer(Allocator.Temp)�̓��������[�N����\�������邽�ߍŌ�ɉ��
        ecb.Dispose();
    }
}
