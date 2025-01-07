using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// �v���C���[�A�G�Ȃǂɗ^����_���[�W�̃o�b�t�@�[
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct DamageBufferElement : IBufferElementData
{
    public int Damage;
}

/// <summary>
/// ���ۂɃ_���[�W��^���鏈���������邽�߂̃f�[�^
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted, OwnerSendType = SendToOwnerType.SendToNonOwner)]
public struct DamageThisTick : ICommandData
{
    public NetworkTick Tick { get; set; }
    public int Damage;
}

/// <summary>
/// �����U�����A�����ē�����Ȃ��悤�ɂ���p�R���|�[�l���g
/// </summary>
public struct AlreadyDamagedEntity : IBufferElementData
{
    public Entity entity;
}

/*[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct AbilityCooldownTargetTick : ICommandData
{
    public NetworkTick Tick { get; set; }
    public NetworkTick Ability;
}*/