using System;
using UnityEngine;

public enum StateType
{
    Idle = 0,
    Equip, 
    Attack, Guard, GuardBroken, Damaged, Stunned, Down, Dead,
}

public class CharacterStateComponent : MonoBehaviour
{
    public event Action<StateType, StateType> OnStateTypeChanged;

    private StateType currentStateType = StateType.Idle;

    public bool IdleState { get => currentStateType == StateType.Idle; }
    public bool EquipState { get => currentStateType == StateType.Equip; }
    public bool AttackState { get => currentStateType == StateType.Attack; }
    public bool GuardState { get => currentStateType == StateType.Guard; }
    public bool GuardBrokenState { get => currentStateType == StateType.GuardBroken; }
    public bool DamagedState { get => currentStateType == StateType.Damaged; }
    public bool StunnedState { get => currentStateType == StateType.Stunned; }
    public bool DownState { get => currentStateType == StateType.Down; }
    public bool DeadState { get => currentStateType == StateType.Dead; }

    public void SetIdleState() => ChangeStateType(StateType.Idle);
    public void SetEquipState() => ChangeStateType(StateType.Equip);
    public void SetAttackState() => ChangeStateType(StateType.Attack);
    public void SetGuardState() => ChangeStateType(StateType.Guard);
    public void SetGuardBrokenState() => ChangeStateType(StateType.GuardBroken);
    public void SetDamagedState() => ChangeStateType(StateType.Damaged);
    public void SetStunnedState() => ChangeStateType(StateType.Stunned);
    public void SetDownState() => ChangeStateType(StateType.Down);
    public void SetDeadState() => ChangeStateType(StateType.Dead);

    private void ChangeStateType(StateType newStateType)
    {
        if (currentStateType == newStateType)
            return;

        StateType prevType = currentStateType;
        currentStateType = newStateType;

        OnStateTypeChanged?.Invoke(prevType, newStateType);
    }
}
