using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;

// �N���C�A���g����уV���N���C�A���g�i�e�X�g�̂��߂�NPC�݂����ȁj�݂̂Ŏ��s
[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
public partial struct ClientRequestGameEntrySystem : ISystem
{
    // �܂��T�[�o�[�ɐڑ����Ă��Ȃ��l�b�g���[�NID�����R���|�[�l���g��T���N�G��
    private EntityQuery _pendingNetworkIdQuery;

    public void OnCreate(ref SystemState state)
    {
        // �ڑ��ҋ@���̃G���e�B�e�B�����鎞����OnUpdate�̏��������s
        var builder = new EntityQueryBuilder(Allocator.Temp).WithAll<NetworkId>().WithNone<NetworkStreamInGame>();
        _pendingNetworkIdQuery = state.GetEntityQuery(builder);
        state.RequireForUpdate(_pendingNetworkIdQuery);
        state.RequireForUpdate<ClientConnectRequest>();
    }

    public void OnUpdate(ref SystemState state)
    {
        // ���N�G�X�g���ꂽ�X�|�[���ꏊ���擾
        var requestedSpawnPosition = SystemAPI.GetSingleton<ClientConnectRequest>().SpawnPosition;
        // �G���e�B�e�B�𑀍�i�R���|�[�l���g�̒ǉ��E�폜�E�ҏW�Ȃǁj���邽�߂̃o�b�t�@�[
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        // �ڑ��ҋ@���̃G���e�B�e�B�����ׂĎ擾
        var pendingNetworkIds = _pendingNetworkIdQuery.ToEntityArray(Allocator.Temp);

        foreach(var pendingNetworkId in pendingNetworkIds)
        {
            // NetworkStreamInGame�R���|�[�l���g��ҋ@���̃G���e�B�e�B�ɒǉ����āA�ڑ���Ԃɂ���
            ecb.AddComponent<NetworkStreamInGame>(pendingNetworkId);

            // �T�[�o�[�ւ̃��N�G�X�g�p�G���e�B�e�B���쐬
            var requestConnectEntity = ecb.CreateEntity();
            // RPC�R�}���h�Ɏ擾�����X�|�[���ꏊ�����߂ăT�[�o�[�֑��M
            ecb.AddComponent(requestConnectEntity, new ConnectRequest { SpawnPositioin = requestedSpawnPosition });
            ecb.AddComponent(requestConnectEntity, new SendRpcCommandRequest { TargetConnection = pendingNetworkId });
        }
        ecb.Playback(state.EntityManager);
    }
}
