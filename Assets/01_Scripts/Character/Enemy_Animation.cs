using UnityEngine;

public partial class Enemy
{
    private void End_Attack()
    {
        animator.SetBool("Attack", false);
    }

    private void Begin_Collision()
    {
        EnableAttackCollider();
    }

    private void End_Collision()
    {
        DisableAttackCollider();
    }
}
