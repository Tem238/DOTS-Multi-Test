using Unity.Entities;
/// <summary>
/// 能力共通のステータスを保持するコンポーネント
/// </summary>
public struct DamageOnTrigger : IComponentData
{
    public int Damage;
}
