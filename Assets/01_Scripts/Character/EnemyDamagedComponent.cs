using UnityEngine;

public enum DamageResult
{
    None = 0,
    Success, Blocked, JustBlocked,
}

public class EnemyDamagedComponent : MonoBehaviour
{
    private CharacterStateComponent stateComponent;

    private void Awake()
    {
        stateComponent = GetComponent<CharacterStateComponent>();
    }

    public DamageResult TryDamage()
    {
        if (stateComponent.GuardState)
        {
            return DamageResult.Blocked;
        }

        return DamageResult.Success;
    }
}
