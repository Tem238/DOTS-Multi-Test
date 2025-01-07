using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Rendering;
using UnityEngine;

/// <summary>
/// �v���C���[�F���^�O
/// </summary>
public struct PlayerTag : IComponentData { }

/// <summary>
/// InitializePlayerSystem�����Ώ۔F���^�O
/// </summary>
public struct NewPlayerTag : IComponentData { }

/// <summary>
/// ����v���C���[�F���^�O
/// </summary>
public struct OwnerPlayerTag : IComponentData { }

/// <summary>
/// �v���C���[�A�G�l�~�[�Ȃǂ̋��ʃX�e�[�^�X
/// </summary>
public struct CommonStatus : IComponentData
{
    public int MaxHP;
    [GhostField] public int HP;
    [GhostField] public float MoveSpeed;
}

/// <summary>
/// �v���C���[�ړ��ʂ̓��͒l
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct PlayerInput : IInputComponentData
{
    // Quantization = 0�Ő��m�ȃf�[�^�𓯊�����
    [GhostField(Quantization = 0)] public float2 MoveValue;
}

/// <summary>
/// �v���C���[�U���̓���
/// </summary>
[GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
public struct AbilityInput : IInputComponentData
{
    [GhostField] public InputEvent Ability1;
    [GhostField] public InputEvent Ability2;
}

public class PlayerAuthoring : MonoBehaviour
{
    // �v���n�u�̃C���X�y�N�^�[����ݒ�ł���悤�ɕύX
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
            // ���I�ɐF��ύX���邽��
            AddComponent<URPMaterialPropertyBaseColor>(entity);
            AddComponent<PlayerInput>(entity);
            AddBuffer<DamageBufferElement>(entity);
            AddBuffer<DamageThisTick>(entity);
            AddComponent<AbilityInput>(entity);
        }
    }
}
