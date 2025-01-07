using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
public partial class CameraMoveSystem : SystemBase
{
    private readonly float3 MAIN_CAMERA_OFFSET = new float3(0, 0, -10);

    private Entity _ownerPlayerEntity;
    private Camera _mainCamera;

    protected override void OnCreate()
    {
        RequireForUpdate<OwnerPlayerTag>();
        RequireForUpdate<MainCameraTag>();
    }

    protected override void OnStartRunning()
    {
        // 操作しているプレイヤーのエンティティをキャッシュ
        _ownerPlayerEntity = SystemAPI.GetSingletonEntity<OwnerPlayerTag>();
        //メインカメラをキャッシュ
        var cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
        _mainCamera = EntityManager.GetComponentObject<MainCamera>(cameraEntity).Value;
    }

    protected override void OnStopRunning()
    {
        _ownerPlayerEntity = Entity.Null;
    }

    protected override void OnUpdate()
    {
        // 操作しているプレイヤーの現在位置を取得
        var playerLocalPosition = EntityManager.GetComponentData<LocalTransform>(_ownerPlayerEntity).Position;
        // オフセットを追加して
        _mainCamera.transform.position = playerLocalPosition + MAIN_CAMERA_OFFSET;
    }
}
