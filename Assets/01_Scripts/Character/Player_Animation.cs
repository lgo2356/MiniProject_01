using UnityEngine;

public partial class Player
{
    private bool isDrawing = false;
    private bool isSheathing = false;
    private bool isEquipped = false;
    private bool isAttacking = false;
    private bool isComboEnabled = false;
    private bool isComboExist = false;

    private int comboIndex = 0;

    public int ComboIndex { get => comboIndex; }

    private void Update_KeyInputEquip()
    {
        if (Input.GetButtonDown("GreateSword") == false)
            return;

        bool check = false;
        check |= (isDrawing);
        check |= (isSheathing);
        check |= (isAttacking);

        if (check)
            return;

        if (isEquipped == false)
        {
            isDrawing = true;
            animator.SetBool("Equip", true);
        }
        else
        {
            isSheathing = true;
            animator.SetBool("Unequip", true);
        }
    }

    private void Update_KeyInputAttack()
    {
        if (Input.GetButtonDown("Attack") == false)
            return;

        if (isEquipped == false)
            return;

        if (isDrawing)
            return;

        if (isSheathing)
            return;

        if (isComboEnabled)
        {
            isComboEnabled = false;
            isComboExist = true;

            return;
        }

        if (isAttacking)
            return;

        moveComponent.enabled = false;
        isAttacking = true;

        if (IsAllowRiposte)
        {
            IsAllowRiposte = false;

            animator.SetBool("Riposte", true);
            OnStartRiposte?.Invoke();

            return;
        }

        if (isCounterAttackTiming)
        {
            isCounterAttackTiming = false;

            animator.SetBool("CounterAttack", true);
        }
        else
        {
            animator.SetBool("Attack", true);
        }
    }

    private void Update_KeyInputShieldBlock()
    {
        if (isDrawing)
            return;

        if (isSheathing)
            return;

        if (isAttacking)
            return;

        if (Input.GetButtonDown("ShieldBlock"))
        {
            animator.SetBool("ShieldBlock", true);
        }
        else if (Input.GetButtonUp("ShieldBlock"))
        {
            animator.SetBool("ShieldBlock", false);

            isShieldBlocking = false;
        }
        else
        {
            return;
        }
    }

    private void ResetFlag()
    {
        isAttacking = false;
        isComboEnabled = false;
        isComboExist = false;

        comboIndex = 0;
    }

    private void Begin_Equip()
    {
        sword.transform.parent.DetachChildren();
        sword.transform.position = Vector3.zero;
        sword.transform.rotation = Quaternion.identity;
        sword.transform.localScale = Vector3.one;
        sword.transform.SetParent(swordSlotTransform, false);
    }

    private void End_Equip()
    {
        isEquipped = true;
        isDrawing = false;

        animator.SetBool("Equip", false);
    }

    private void Begin_Unequip()
    {
        sword.transform.parent.DetachChildren();
        sword.transform.position = Vector3.zero;
        sword.transform.rotation = Quaternion.identity;
        sword.transform.localScale = Vector3.one;
        sword.transform.SetParent(swordHolsterTransform, false);
    }

    private void End_Unequip()
    {
        isEquipped = false;
        isSheathing = false;

        animator.SetBool("Unequip", false);
    }

    #region Attack Animation Events
    private void Begin_Combo()
    {
        isComboEnabled = true;
    }

    private void End_Combo()
    {
        isComboEnabled = false;
    }

    private void Begin_Attack()
    {
        if (isComboExist == false)
            return;

        isComboExist = false;

        comboIndex++;
        animator.SetTrigger("DoNextCombo");
    }

    private void End_Attack()
    {
        comboIndex = 0;

        moveComponent.enabled = true;

        isAttacking = false;
        animator.SetBool("Attack", false);
    }

    private void Begin_CounterAttack()
    {
        sword.IsCritical = true;
    }

    private void End_CounterAttack()
    {
        moveComponent.enabled = true;

        isAttacking = false;

        sword.IsCritical = false;

        animator.SetBool("CounterAttack", false);
    }

    private void Begin_Riposte()
    {
        IsRiposte = true;
    }

    private void End_Riposte()
    {
        moveComponent.enabled = true;

        isAttacking = false;
        IsRiposte = false;

        animator.SetBool("Riposte", false);
    }

    private void Begin_Collision()
    {
        sword.EnableCollision();
    }

    private void End_Collision()
    {
        sword.DisableCollision();
    }
    #endregion

    #region Hit Animation Events
    private void Begin_Hit()
    {
        moveComponent.enabled = false;
    }

    private void End_Hit()
    {
        moveComponent.enabled = true;
    }
    #endregion

    private void Begin_ShieldBlock()
    {
        isShieldBlocking = true;
    }

    private void Begin_JustGuard()
    {
        isJustGuarding = true;
    }

    private void End_JustGuard()
    {
        isJustGuarding = false;
    }
}
