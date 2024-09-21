using System.Collections;
using UnityEngine;

/**
 * ������Ʈ�� Ȱ��ȭ �Ǿ� �ְ� ������ ������ ������ �����̴� ����
 */
public class EnemyMoveComponent : MonoBehaviour
{
    private Animator animator;

    private Vector3 destination;
    private GameObject lookTarget;
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

            Vector3 lookDirection;

            if (lookTarget != null)
            {
                lookDirection = lookTarget.transform.position - transform.position;
            }
            else
            {
                lookDirection = destination - transform.position;
            }
            
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
        enabled = true;

        this.destination = destination;
    }

    public void SetMove(Vector3 destination, float moveSpeed)
    {
        SetMove(destination);

        this.moveSpeed = moveSpeed;
    }

    public void SetMove(Vector3 destination, float moveSpeed, GameObject lookTarget)
    {
        SetMove(destination, moveSpeed);

        this.lookTarget = lookTarget;
    }

    public void StopMove()
    {
        enabled = false;

        destination = transform.position;
        moveSpeed = 2f;
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(destination, 0.25f);
    }
}
