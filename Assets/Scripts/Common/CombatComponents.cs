using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// プレイヤー、敵などに与えるダメージのバッファー
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct DamageBufferElement : IBufferElementData
{
    public int Damage;
}

/// <summary>
/// 実際にダメージを与える処理をさせるためのデータ
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted, OwnerSendType = SendToOwnerType.SendToNonOwner)]
public struct DamageThisTick : ICommandData
{
    public NetworkTick Tick { get; set; }
    public int Damage;
}

/// <summary>
/// 同じ攻撃が連続して当たらないようにする用コンポーネント
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