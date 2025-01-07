using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Rendering;
using UnityEngine;

/// <summary>
/// プレイヤー認識タグ
/// </summary>
public struct PlayerTag : IComponentData { }

/// <summary>
/// InitializePlayerSystem処理対象認識タグ
/// </summary>
public struct NewPlayerTag : IComponentData { }

/// <summary>
/// 操作プレイヤー認識タグ
/// </summary>
public struct OwnerPlayerTag : IComponentData { }

/// <summary>
/// プレイヤー、エネミーなどの共通ステータス
/// </summary>
public struct CommonStatus : IComponentData
{
    public int MaxHP;
    [GhostField] public int HP;
    [GhostField] public float MoveSpeed;
}

/// <summary>
/// プレイヤー移動量の入力値
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct PlayerInput : IInputComponentData
{
    // Quantization = 0で正確なデータを同期する
    [GhostField(Quantization = 0)] public float2 MoveValue;
}

/// <summary>
/// プレイヤー攻撃の入力
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct AbilityInput : IInputComponentData
{
    [GhostField] public InputEvent Ability1;
    [GhostField] public InputEvent Ability2;
}

public class PlayerAuthoring : MonoBehaviour
{
    // プレハブのインスペクターから設定できるように変更
    public int HP;
    public float MoveSpeed;

    public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
    {
        public override void Bake(PlayerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<PlayerTag>(entity);
            AddComponent<NewPlayerTag>(entity);
            AddComponent(entity, new CommonStatus
            {
                MaxHP = authoring.HP,
                HP = authoring.HP,
                MoveSpeed = authoring.MoveSpeed,
            });
            // 動的に色を変更するため
            AddComponent<URPMaterialPropertyBaseColor>(entity);
            AddComponent<PlayerInput>(entity);
            AddBuffer<DamageBufferElement>(entity);
            AddBuffer<DamageThisTick>(entity);
            AddComponent<AbilityInput>(entity);
        }
    }
}
