using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct AbilityAspect : IAspect
{
    private readonly RefRO<AbilityInput> _abilityInput;
    private readonly DynamicBuffer<AbilitySlotBufferElement> _abilitySlots;
    private readonly RefRO<LocalTransform> _localTransform;
    private readonly DynamicBuffer<AbilityBufferElement> _abilities;

    /// <summary>
    /// 能力の有効・無効を切り替えるスイッチ
    /// </summary>
    /// <param name="key">入力キー</param>
    /// <returns>入力キーが押されたか</returns>
    public bool GetToggle(int key)
    {
        return key switch
        {
            1 => _abilityInput.ValueRO.Ability1.IsSet,
            2 => _abilityInput.ValueRO.Ability2.IsSet,
            _ => throw new ArgumentOutOfRangeException(nameof(key), "無効な入力キーです：" + nameof(GetToggle))
        };
    }

    /// <summary>
    /// 能力一覧から
    /// </summary>
    /// <param name="abilityIndex">能力番号</param>
    /// <returns>能力のデータ</returns>
    public AbilityBufferElement GetAbility(int abilityIndex)
    {
        foreach(var ability in _abilities)
        {
            if(ability.AbilityIndex == abilityIndex)
            {
                return ability;
            }
        }
        throw new IndexOutOfRangeException($"能力番号{abilityIndex}は存在しません：{nameof(GetAbility)}");
    }

    /// <summary>
    /// アビリティ1を持ったプレイヤーの位置
    /// </summary>
    public float3 AttackPosition => _localTransform.ValueRO.Position;

    public DynamicBuffer<AbilitySlotBufferElement> AbilitySlots => _abilitySlots;

    /*/// <summary>
    /// BLOBアセットから能力のデータを取得
    /// </summary>
    /// <param name="index">取得するインデックス番号</param>
    /// <returns>能力データ</returns>
    public Ability GetAbility(int? index)
    {
        if (!_abilities.ValueRO.Blob.IsCreated)
        {
            throw new InvalidOperationException("BlobAssetReference is not created.");
        }
        // Get a reference to the pool of available hobbies. Note that it needs to be passed by
        // reference, because otherwise the internal reference in the BlobArray would be invalid.
        ref AbilityPool pool = ref _abilities.ValueRO.Blob.Value;

        if (!index.HasValue || index < 0 || index >= pool.Abilities.Length)
        {
            throw new IndexOutOfRangeException("Index is out of range.");
        }

        return pool.Abilities[index.Value];
    }*/
}
