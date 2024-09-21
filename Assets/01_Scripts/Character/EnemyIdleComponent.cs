using System.Collections;
using UnityEngine;

public class EnemyIdleComponent : MonoBehaviour
{
    private Coroutine idleCoroutine;

    public void StartIdle()
    {
        // ���� ����
        //idleCoroutine = StartCoroutine(Coroutine_Idle());
    }

    public void StopIdle()
    {
        StopCoroutine(idleCoroutine);
    }

    private IEnumerator Coroutine_Idle()
    {
        // 1. �ֺ��� �� ã�� (Scan)
        // 2-1. ���� ������ �Ӱ� �Ÿ����� ���� (Chase)
        // 2-2. ���� ������ ���� (Patrol)

        yield return null;
    }
}
