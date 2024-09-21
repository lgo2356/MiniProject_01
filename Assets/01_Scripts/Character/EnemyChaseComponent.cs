using System.Collections;
using UnityEngine;

public class EnemyChaseComponent : MonoBehaviour
{
    [SerializeField]
    private float criticalRange = 1.2f;

    private EnemyMoveComponent moveComponent;
    private EnemyCombatComponent combatComponent;

    private Coroutine chaseCoroutine;

    private void Awake()
    {
        moveComponent = GetComponent<EnemyMoveComponent>();
        combatComponent = GetComponent<EnemyCombatComponent>();
    }

    public void StartChase(GameObject target)
    {
        chaseCoroutine = StartCoroutine(Coroutine_ChasePlayer(target));
    }

    public void StopChase()
    {
        StopCoroutine(chaseCoroutine);
    }

    private IEnumerator Coroutine_ChasePlayer(GameObject target)
    {
        WaitForSeconds wait = new(2f);

        while (true)
        {
            if (Vector3.Distance(target.transform.position, transform.position) <= criticalRange)
            {
                moveComponent.StopMove();

                StopChase();

                //Attack();
                //TODO: Chase -> Idle -> Combat
                combatComponent.StartCombat(target);

                yield return wait;
            }
            else
            {
                moveComponent.SetMove(target.transform.position, 2f);
            }

            yield return null;
        }
    }
}
