using Unity.Entities;
using UnityEngine;

// メインカメラの参照を保持するコンポーネント
// Cameraはマネージドクラスのため、structではなくclassで定義
public class MainCamera : IComponentData
{
    public Camera Value;
}

// メインカメラ認識用
public struct MainCameraTag : IComponentData { }

public class MainCameraAuthoring : MonoBehaviour
{
    public class Baker : Baker<MainCameraAuthoring>
    {
        public override void Bake(MainCameraAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            // マネージドクラスはAddComponentObjectで追加する
            AddComponentObject(entity, new MainCamera());
            AddComponent<MainCameraTag>(entity);
        }
    }
}
