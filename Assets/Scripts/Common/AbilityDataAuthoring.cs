using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public struct AbilityBufferElement : IBufferElementData
{
    public int AbilityIndex;
    public Entity AbilityPrefab;
    public int Damage;
    public uint CoolDownTick;
    public float DestroyOnTimer;
}

[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct AbilityBeginBufferElement : IBufferElementData
{
    public int AbilityIndex;
}

public class AbilityDataAuthoring : MonoBehaviour
{
    public List<AbilityData> Abilities;
    public NetCodeConfig NetCodeConfig;
    private int _simulationTickRate => NetCodeConfig.ClientServerTickRate.SimulationTickRate;

    public class Baker : Baker<AbilityDataAuthoring>
    {
        public override void Bake(AbilityDataAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddBuffer<AbilityBeginBufferElement>(entity);
            var buffer = AddBuffer<AbilityBufferElement>(entity);
            foreach(var ability in authoring.Abilities)
            {
                buffer.Add(new AbilityBufferElement
                {
                    AbilityIndex = ability.AbilityIndex,
                    AbilityPrefab = GetEntity(ability.AbilityPrefab, TransformUsageFlags.Dynamic),
                    Damage = ability.Damage,
                    CoolDownTick = (uint)(ability.CoolDownTime * authoring._simulationTickRate),
                    DestroyOnTimer = ability.DestroyOnTimer,
                });
            }
        }
    }
}

/*/// <summary>
/// 能力のデータ
/// </summary>
public struct Ability
{
    public Entity AbilityPrefab;
    public int Damage;
    public float CoolDownTime;
    public float DestroyOnTimer;
}

/// <summary>
/// 能力のデータを複数保持するための配列
/// </summary>
public struct AbilityPool
{
    public BlobArray<Ability> Abilities;
}

/// <summary>
/// 能力データの参照を保持するコンポーネント
/// </summary>
public struct Abilities : IComponentData
{
    public BlobAssetReference<AbilityPool> Blob;
}

public class AbilityDataAuthoring : MonoBehaviour
{
    public List<AbilityData> Abilities;

    public class Baker : Baker<AbilityDataAuthoring>
    {
        public override void Bake(AbilityDataAuthoring authoring)
        {
            var builder = new BlobBuilder(Allocator.Temp);
            ref AbilityPool abilityPool = ref builder.ConstructRoot<AbilityPool>();
            var ability = authoring.Abilities;

            // Allocate enough room for two hobbies in the pool. Use the returned BlobBuilderArray
            // to fill in the data.
            BlobBuilderArray<Ability> arrayBuilder = builder.Allocate(
                ref abilityPool.Abilities,
                ability.Count
            );

            // Initialize the hobbies.

            // An exciting hobby that consumes a lot of oranges.
            for (var i = 0; i < ability.Count; i++)
            {
                arrayBuilder[i] = new Ability
                {
                    AbilityPrefab = GetEntity(ability[i].AbilityPrefab, TransformUsageFlags.Dynamic),
                    Damage = ability[i].Damage,
                    CoolDownTime = ability[i].CoolDownTime,
                    DestroyOnTimer = ability[i].DestroyOnTimer
                };
            }

            var blobReference = builder.CreateBlobAssetReference<AbilityPool>(Allocator.Persistent);
            // Register the Blob Asset to the Baker for de-duplication and reverting.
            AddBlobAsset<AbilityPool>(ref blobReference, out _);
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new Abilities() { Blob = blobReference });

            // Make sure to dispose the builder itself so all internal memory is disposed.
            builder.Dispose();
        }
    }
}*/
