using System.Collections;
using UnityEngine;

public class EnemyPatrolComponent : MonoBehaviour
{
    private EnemyMoveComponent moveComponent;

    private Coroutine patrolCoroutine;

    private void Awake()
    {
        moveComponent = GetComponent<EnemyMoveComponent>();
    }

    public void StartPatrol(float preDelay = 0f)
    {
        patrolCoroutine = StartCoroutine(Coroutine_Patrol(preDelay));
    }

    public void StopPatrol()
    {
        StopCoroutine(patrolCoroutine);
    }

    private IEnumerator Coroutine_Patrol(float preDelay = 0f)
    {
        yield return new WaitForSeconds(preDelay);

        WaitForFixedUpdate wait = new();

        bool isFirst = true;

        moveComponent.SetMove(transform.position);

        while (true)
        {
            if (Vector3.Distance(moveComponent.Destination, transform.position) < 0.1f)
            {
                float waitTime = UnityEngine.Random.Range(1f, 3f);

                if (isFirst)
                {
                    isFirst = false;
                    waitTime = 0f;
                }

                yield return new WaitForSeconds(waitTime);

                moveComponent.SetMove(GetRandomPosition(), 1f);
            }

            yield return wait;
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x = UnityEngine.Random.Range(-10f, 10f);
        float z = UnityEngine.Random.Range(-8.5f, 8.5f);

        return new Vector3(x, 0f, z);
    }
}
