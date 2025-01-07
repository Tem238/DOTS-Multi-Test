using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 能力1特有のステータスを保持するコンポーネント
/// </summary>
public struct Ability1Status : IComponentData
{
    public float3 Forward;
    public float MoveSpeed;
}

public class Ability1Authoring : MonoBehaviour
{
    public float MoveSpeed = 3.0f;

    public class Baker : Baker<Ability1Authoring>
    {
        public override void Bake(Ability1Authoring authoring)
        {
            AddComponent(GetEntity(TransformUsageFlags.None), new Ability1Status
            {
                Forward = new float3(1, 0, 0),
                MoveSpeed = authoring.MoveSpeed,
            });
        }
    }
}
