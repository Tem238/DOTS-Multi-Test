using Unity.Entities;
using UnityEngine;

public struct EnemyPrefabs : IComponentData
{
    public Entity Enemy1;
}

class EnemyPrefabsAuthoring : MonoBehaviour
{
    public GameObject Enemy1;

    public class Baker : Baker<EnemyPrefabsAuthoring>
    {
        public override void Bake(EnemyPrefabsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EnemyPrefabs
            {
                Enemy1 = GetEntity(authoring.Enemy1, TransformUsageFlags.Dynamic)
            });
        }
    }
}
