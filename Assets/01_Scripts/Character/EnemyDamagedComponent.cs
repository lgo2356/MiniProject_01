using UnityEngine;

public enum DamageResult
{
    None = 0,
    Success, Blocked, JustBlocked, Dead,
}

public class EnemyDamagedComponent : MonoBehaviour
{
    private Animator animator;
    private CharacterStateComponent stateComponent;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
