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
    /// �\�̗͂L���E������؂�ւ���X�C�b�`
    /// </summary>
    /// <param name="key">���̓L�[</param>
    /// <returns>���̓L�[�������ꂽ��</returns>
    public bool GetToggle(int key)
    {
        return key switch
        {
            1 => _abilityInput.ValueRO.Ability1.IsSet,
            2 => _abilityInput.ValueRO.Ability2.IsSet,
            _ => throw new ArgumentOutOfRangeException(nameof(key), "�����ȓ��̓L�[�ł��F" + nameof(GetToggle))
        };
    }

    /// <summary>
    /// �\�͈ꗗ����
    /// </summary>
    /// <param name="abilityIndex">�\�͔ԍ�</param>
    /// <returns>�\�͂̃f�[�^</returns>
    public AbilityBufferElement GetAbility(int abilityIndex)
    {
        foreach(var ability in _abilities)
        {
            if(ability.AbilityIndex == abilityIndex)
            {
                return ability;
            }
        }
        throw new IndexOutOfRangeException($"�\�͔ԍ�{abilityIndex}�͑��݂��܂���F{nameof(GetAbility)}");
    }

    /// <summary>
    /// �A�r���e�B1���������v���C���[�̈ʒu
    /// </summary>
    public float3 AttackPosition => _localTransform.ValueRO.Position;

    public DynamicBuffer<AbilitySlotBufferElement> AbilitySlots => _abilitySlots;

    /*/// <summary>
    /// BLOB�A�Z�b�g����\�͂̃f�[�^���擾
    /// </summary>
    /// <param name="index">�擾����C���f�b�N�X�ԍ�</param>
    /// <returns>�\�̓f�[�^</returns>
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
