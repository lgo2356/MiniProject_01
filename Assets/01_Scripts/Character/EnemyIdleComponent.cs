using System.Collections;
using UnityEngine;

public class EnemyIdleComponent : MonoBehaviour
{
    private Coroutine idleCoroutine;

    public void StartIdle()
    {
        // 순찰 시작
        //idleCoroutine = StartCoroutine(Coroutine_Idle());
    }

    public void StopIdle()
    {
        StopCoroutine(idleCoroutine);
    }

    private IEnumerator Coroutine_Idle()
    {
        // 1. 주변의 적 찾기 (Scan)
        // 2-1. 적이 있으면 임계 거리까지 접근 (Chase)
        // 2-2. 적이 없으면 순찰 (Patrol)

        yield return null;
    }
}
