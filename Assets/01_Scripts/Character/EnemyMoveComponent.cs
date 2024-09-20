using System.Collections;
using UnityEngine;

/**
 * 컴포넌트가 활성화 되어 있고 목적지 정보가 있으면 움직이는 로직
 */
public class EnemyMoveComponent : MonoBehaviour
{
    public Vector3 Destination;
    public float MoveSpeed;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        Destination = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(Destination, transform.position) < 0.1f)
        {
            animator.SetFloat("SpeedZ", 0f);
        }
        else
        {
            animator.SetFloat("SpeedZ", MoveSpeed);

            Vector3 lookDirection = Destination - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);

            transform.rotation = lookRotation;
            transform.position += transform.forward * MoveSpeed * Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        animator.SetFloat("SpeedZ", 0f);

        Destination = transform.position;
    }

    private IEnumerator Coroutine_Enable(float delay)
    {
        yield return new WaitForSeconds(delay);

        enabled = true;
    }

    private Vector3 GetRandomPosition()
    {
        float x = UnityEngine.Random.Range(-10f, 10f);
        float z = UnityEngine.Random.Range(-8.5f, 8.5f);

        return new Vector3(x, 0f, z);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Destination, 0.25f);
    }
}
