using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

// このグループはクライアント側でのみ実行される
[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class PlayerMoveInputSystem : SystemBase
{
    // InputSystemで設定したアクション
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
        // プレイヤーの入力をコンポーネントにセット
        EntityManager.SetComponentData(_ownerPlayerEntity, new PlayerInput
        {
            MoveValue = _inputActions.GameplayMap.PlayerMovement.ReadValue<Vector2>()
        });
    }

}
