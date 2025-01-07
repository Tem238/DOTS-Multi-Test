using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

// �T�[�o�[�ł̂ݎ��s
[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
public partial struct ServerProcessGameEntryRequestSystem : ISystem
{
    // �T�[�o�[�ւ̒ʐM(RPC���N�G�X�g)������ꍇ�̂ݎ��s
    public void OnCreate(ref SystemState state)
    {
        // �N���C�A���g�����M����SendRpcCommandRequest���T�[�o�[��ReceiveRpcCommandRequest�Ŏ󂯎��
        var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<ConnectRequest, ReceiveRpcCommandRequest>();
        state.RequireForUpdate(state.GetEntityQuery(builder));
        state.RequireForUpdate<PlayerPrefabs>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var playerPrefab = SystemAPI.GetSingleton<PlayerPrefabs>().Player;

        foreach(var (connectRequest, requestSource, requestEntity) in 
            SystemAPI.Query<ConnectRequest, ReceiveRpcCommandRequest>().WithEntityAccess())
        {
            // RPC���N�G�X�g��1�񂾂������������̂ŁA���̃��N�G�X�g�̃G���e�B�e�B�͍폜����
            ecb.DestroyEntity(requestEntity);
            ecb.AddComponent<NetworkStreamInGame>(requestSource.SourceConnection);

            // �N���C�A���g�̃l�b�g���[�NID���擾
            var clientId = SystemAPI.GetComponent<NetworkId>(requestSource.SourceConnection).Value;
            Debug.Log($"�T�[�o�[�ɐڑ����܂����B ID:{clientId}");

            // �v���C���[�̐���
            var newPlayer = ecb.Instantiate(playerPrefab);
            // �킩��₷���悤�ɃI�u�W�F�N�g�̖��O��t���Ă����܂�
            ecb.SetName(newPlayer, "Player");
            // ���N�G�X�g���ꂽ�ꏊ�Ɉړ�
            var newTransform = LocalTransform.FromPosition(connectRequest.SpawnPositioin);
            ecb.SetComponent(newPlayer, newTransform);
            // �S�[�X�g�i�N���C�A���g�ɕ\���������I�u�W�F�N�g�j�̏��L�҂𐶐������N�G�X�g�����N���C�A���g�Ɏw��
            ecb.SetComponent(newPlayer, new GhostOwner { NetworkId = clientId });

            ecb.AppendToBuffer(requestSource.SourceConnection, new LinkedEntityGroup { Value = newPlayer });
        }
        ecb.Playback(state.EntityManager);
    }
}
