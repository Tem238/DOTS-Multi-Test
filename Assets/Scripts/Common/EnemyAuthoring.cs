using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// エネミー認識タグ
/// </summary>
public struct EnemyTag : IComponentData { }

/// <summary>
/// 新規エネミー初期化用タグ
/// </summary>
public struct NewEnemyTag : IComponentData { }

/// <summary>
/// エネミーが向かっていく場所
/// </summary>
public struct TargetPosition : IComponentData
{
    [GhostField] public float3 position;
}

public class EnemyAuthoring : MonoBehaviour
{
    public int HP;
    public float MoveSpeed;

    public class Baker : Baker<EnemyAuthoring>
    {
        public override void Bake(EnemyAuthoring enemyAuthoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<EnemyTag>(entity);
            AddComponent<NewEnemyTag>(entity);
            AddComponent(entity, new TargetPosition
            {
                position = float3.zero,
            });
            AddComponent(entity, new CommonStatus
            {
                MaxHP = enemyAuthoring.HP,
                HP = enemyAuthoring.HP,
                MoveSpeed = enemyAuthoring.MoveSpeed,
            });
            AddBuffer<DamageBufferElement>(entity);
            AddBuffer<DamageThisTick>(entity);
            
        }
    }
}
