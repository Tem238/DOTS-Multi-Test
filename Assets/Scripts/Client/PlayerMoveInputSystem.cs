using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

// ���̃O���[�v�̓N���C�A���g���ł̂ݎ��s�����
[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class PlayerMoveInputSystem : SystemBase
{
    // InputSystem�Őݒ肵���A�N�V����
    private TestInputAction _inputActions;
    private Entity _ownerPlayerEntity;

    protected override void OnCreate()
    {
        RequireForUpdate<OwnerPlayerTag>();
        _inputActions = new TestInputAction();
    }

    protected override void OnStartRunning()
    {
        _inputActions.Enable();
        _ownerPlayerEntity = SystemAPI.GetSingletonEntity<OwnerPlayerTag>();
    }

    protected override void OnStopRunning()
    {
        _inputActions.Disable();
        _ownerPlayerEntity = Entity.Null;
    }
    protected override void OnUpdate()
    {
        // �v���C���[�̓��͂��R���|�[�l���g�ɃZ�b�g
        EntityManager.SetComponentData(_ownerPlayerEntity, new PlayerInput
        {
            MoveValue = _inputActions.GameplayMap.PlayerMovement.ReadValue<Vector2>()
        });
    }

}
