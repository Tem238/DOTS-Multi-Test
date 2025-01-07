using Unity.Entities;
using Unity.NetCode;

public struct DestroyOnTimer : IComponentData
{
    public float Timer;
}

public struct DestroyAtTick : IComponentData
{
    [GhostField] public NetworkTick Tick;
}

public struct DestroyEntityTag : IComponentData { }
