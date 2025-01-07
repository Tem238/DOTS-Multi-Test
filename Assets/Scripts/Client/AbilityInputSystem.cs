using Unity.Entities;
using Unity.NetCode;

// このグループはクライアント側でのみ実行される
[UpdateInGroup(typeof(GhostInputSystemGroup))]
public partial class AbilityInputSystem : SystemBase
{
    private TestInputAction _inputAction;

    protected override void OnCreate()
    {
        _inputAction = new TestInputAction();
    }

    protected override void OnStartRunning()
    {
        _inputAction.Enable();
    }

    protected override void OnStopRunning()
    {
        _inputAction.Disable();
    }

    protected override void OnUpdate()
    {
        var newAbilityInput = new AbilityInput();

        if (_inputAction.GameplayMap.Ability1.WasPressedThisFrame())
        {
            newAbilityInput.Ability1.Set();
        }

        if (_inputAction.GameplayMap.Ability2.WasPressedThisFrame())
        {
            newAbilityInput.Ability2.Set();
        }

        foreach (var abilityInput in SystemAPI.Query<RefRW<AbilityInput>>())
        {
            abilityInput.ValueRW = newAbilityInput;
        }
    }
}
