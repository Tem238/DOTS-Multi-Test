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
        // ���삵�Ă���v���C���[�̃G���e�B�e�B���L���b�V��
        _ownerPlayerEntity = SystemAPI.GetSingletonEntity<OwnerPlayerTag>();
        //���C���J�������L���b�V��
        var cameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
        _mainCamera = EntityManager.GetComponentObject<MainCamera>(cameraEntity).Value;
    }

    protected override void OnStopRunning()
    {
        _ownerPlayerEntity = Entity.Null;
    }

    protected override void OnUpdate()
    {
        // ���삵�Ă���v���C���[�̌��݈ʒu���擾
        var playerLocalPosition = EntityManager.GetComponentData<LocalTransform>(_ownerPlayerEntity).Position;
        // �I�t�Z�b�g��ǉ�����
        _mainCamera.transform.position = playerLocalPosition + MAIN_CAMERA_OFFSET;
    }
}
