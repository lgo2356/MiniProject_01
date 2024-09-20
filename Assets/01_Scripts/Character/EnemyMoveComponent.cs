using System.Collections;
using UnityEngine;

/**
 * 컴포넌트가 활성화 되어 있고 목적지 정보가 있으면 움직이는 로직
 */
public class EnemyMoveComponent : MonoBehaviour
{
    private Animator animator;

    private Vector3 destination;
    private float moveSpeed;

    public Vector3 Destination => destination;
    public float MoveSpeed => moveSpeed;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        destination = transform.position;
    }

    private void Update()
    {
        if (Vector3.Distance(destination, transform.position) < 0.1f)
        {
            animator.SetFloat("SpeedZ", 0f);
        }
        else
        {
            animator.SetFloat("SpeedZ", moveSpeed);

            Vector3 lookDirection = destination - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);

            transform.rotation = lookRotation;
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        animator.SetFloat("SpeedZ", 0f);

        destination = transform.position;
    }

    public void SetMove(Vector3 destination)
    {
        this.destination = destination;
    }

    public void SetMove(Vector3 destination, float moveSpeed)
    {
        this.destination = destination;
        this.moveSpeed = moveSpeed;
    }

    public void StopMove()
    {
        destination = transform.position;
        moveSpeed = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(destination, 0.25f);
    }
}
