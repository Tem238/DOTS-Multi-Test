using Unity.Entities;
using UnityEngine;

// ���C���J�����̎Q�Ƃ�ێ�����R���|�[�l���g
// Camera�̓}�l�[�W�h�N���X�̂��߁Astruct�ł͂Ȃ�class�Œ�`
public class MainCamera : IComponentData
{
    public Camera Value;
}

// ���C���J�����F���p
public struct MainCameraTag : IComponentData { }

public class MainCameraAuthoring : MonoBehaviour
{
    public class Baker : Baker<MainCameraAuthoring>
    {
        public override void Bake(MainCameraAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            // �}�l�[�W�h�N���X��AddComponentObject�Œǉ�����
            AddComponentObject(entity, new MainCamera());
            AddComponent<MainCameraTag>(entity);
        }
    }
}
