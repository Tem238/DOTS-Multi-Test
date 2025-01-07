using Unity.Entities;
using UnityEngine;

public struct PlayerPrefabs : IComponentData
{
    public Entity Player;
}

class PlayerPrefabsAuthoring : MonoBehaviour
{
    public GameObject Player;

    public class PlayerPrefabsAuthoringBaker : Baker<PlayerPrefabsAuthoring>
    {
        public override void Bake(PlayerPrefabsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayerPrefabs
            {
                Player = GetEntity(authoring.Player, TransformUsageFlags.Dynamic)
            });
        }
    }
}
