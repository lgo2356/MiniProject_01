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

    private void Update_InputEquip()
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

    private void Update_InputAttack()
    {
        if (Input.GetButtonDown("Attack") == false)
            return;

        //bool check = false;
        //check |= (isEquipped == false);
        //check |= (isDrawing);
        //check |= (isSheathing);

        if (isEquipped == false)
            return;

        if (isDrawing)
            return;

        if (isSheathing)
            return;

        //if (check == false)
        //    return;

        if (isComboEnabled)
        {
            isComboEnabled = false;
            isComboExist = true;

            return;
        }

        if (isAttacking)
            return;

        moveState.enabled = false;
        isAttacking = true;

        animator.SetBool("Attack", true);
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

        moveState.enabled = true;

        isAttacking = false;
        animator.SetBool("Attack", false);
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
        //animator.SetBool("Hit", false);
    }

    private void End_Hit()
    {
        
    }
    #endregion
}
