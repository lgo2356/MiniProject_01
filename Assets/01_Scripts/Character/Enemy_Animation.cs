using UnityEngine;

public partial class Enemy
{
    private bool isComboEnabled = false;
    private bool isComboExist = false;

    private int comboIndex = 0;

    public int ComboIndex { get => comboIndex; }

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
        if (stateComponent.AttackState == false)
            return;

        sword.EnableCollision();
    }

    private void End_Collision()
    {
        sword.DisableCollision();
    }

    private void Begin_Stunned()
    {
        //sword.DisableCollision();
    }

    private void Begin_Riposte()
    {
        GameObject go = GameObject.Find("Player");
        Player player = go.GetComponent<Player>();
        player.OnStartVictim?.Invoke();
    }

    private void End_Riposte()
    {
        animator.SetBool("Riposte", false);
    }

    private void Riposte_Hit()
    {
        hpComponent.Damage(201f);
    }

    private void End_RiposteVictim()
    {
        if (hpComponent.IsDead)
        {
            gameObject.GetComponent<Collider>().enabled = false;

            OnDead?.Invoke(this);
        }
        else
        {
            Debug.Log("No");
        }
    }
}
