using System.Collections;
using UnityEngine;

public class EnemyChaseComponent : MonoBehaviour
{
    private EnemyScanComponent scanComponent;
    private EnemyMoveComponent moveComponent;

    private Coroutine chaseCoroutine;

    private void Awake()
    {
        scanComponent = GetComponent<EnemyScanComponent>();
        moveComponent = GetComponent<EnemyMoveComponent>();
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
        WaitForSeconds wait = new WaitForSeconds(2f);

        while (true)
        {
            if (Vector3.Distance(target.transform.position, transform.position) <= scanComponent.AttackRange)
            {
                moveComponent.StopMove();

                //Attack();

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
