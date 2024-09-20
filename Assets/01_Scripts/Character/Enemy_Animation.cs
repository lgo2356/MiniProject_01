using UnityEngine;

public partial class Enemy
{
    private bool isComboEnabled = false;
    private bool isComboExist = false;

    private int comboIndex = 0;

    public int ComboIndex { get => comboIndex; }

    private void Begin_Attack()
    {
        if (isComboExist == false)
            return;

        //isComboExist = false;

        comboIndex++;
        animator.SetTrigger("DoNextCombo");
    }

    private void End_Attack()
    {
        comboIndex = 0;
        animator.SetBool("Attack", false);
    }

    private void Begin_Combo()
    {
        isComboEnabled = true;
    }

    private void End_Combo()
    {
        isComboEnabled = false;
    }

    private void Begin_Collision()
    {
        sword.EnableCollision();
    }

    private void End_Collision()
    {
        sword.DisableCollision();
    }
}
