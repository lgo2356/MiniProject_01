using System.Collections;
using UnityEngine;

public class EnemyCombatComponent : MonoBehaviour
{
    private Animator animator;
    private CharacterStateComponent stateComponent;
    private EnemyMoveComponent moveComponent;

    private Coroutine hoveringCoroutine;

    private GameObject combatTarget;
    private bool isHovering = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        moveComponent = GetComponent<EnemyMoveComponent>();
        stateComponent = GetComponent<CharacterStateComponent>();
    }

    public void StartCombat(GameObject target)
    {
        Attack();
        //Guard();
        //StartHovering(target);
    }

    public void StopCombat()
    {
        combatTarget = null;
    }

    //TODO: 랜덤으로 공격이나 방어를 한다.
    private void Attack()
    {
        animator.SetBool("IsAttack", true);
    }

    private void Guard()
    {
        animator.SetBool("IsShield", true);
    }

    private void StartHovering(GameObject target)
    {
        isHovering = true;

        StartCoroutine(Coroutine_Hovering(target));
    }

    private void StopHovering()
    {
        isHovering = false;

        StopCoroutine(hoveringCoroutine);
        hoveringCoroutine = null;
    }

    private IEnumerator Coroutine_Hovering(GameObject target)
    {
        while (isHovering)
        {
            Vector3 lookDirection = target.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);

            transform.rotation = lookRotation;
            transform.position += transform.right * 0.3f * Time.deltaTime;

            yield return null;
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x = UnityEngine.Random.Range(-1f, 1f);
        float z = UnityEngine.Random.Range(-0f, 0f);

        return new Vector3(x, 0f, z);
    }

    #region Animation Event Method
    private void Begin_Attack()
    {
        //if (isComboExist == false)
        //    return;

        //isComboExist = false;

        //comboIndex++;
        //animator.SetTrigger("DoNextCombo");
    }

    private void End_Attack()
    {
        //comboIndex = 0;
        //animator.SetBool("IsAttack", false);
    }

    private void Begin_Shield()
    {
        stateComponent.SetGuardState();
    }

    private void End_Shield()
    {
        stateComponent.SetIdleState();
    }

    private void Begin_JustGuard()
    {

    }

    private void End_JustGuard()
    {

    }
    #endregion
}
