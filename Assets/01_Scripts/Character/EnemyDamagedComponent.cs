using System.Collections;
using UnityEditor;
using UnityEngine;

public enum DamageResult
{
    None = 0,
    Success, Blocked, JustBlocked,
}

public class EnemyDamagedComponent : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    private Animator animator;
    private CharacterStateComponent stateComponent;

    private Collider[] closetTargets = new Collider[1];
    private GameObject closetTarget;

    #region Properties
    public bool IsAllowRiposte => closetTarget != null;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateComponent = GetComponent<CharacterStateComponent>();
    }

    private void Start()
    {
        stateComponent.OnStateTypeChanged += (prevState, newState) =>
        {
            if (newState == StateType.Stunned)
            {
                StartCoroutine(Coroutine_AllowRiposte());
            }
        };
    }

    public DamageResult TryDamage()
    {
        if (stateComponent.GuardState)
        {
            return DamageResult.Blocked;
        }

        return DamageResult.Success;
    }

    private IEnumerator Coroutine_AllowRiposte()
    {
        while (closetTarget == null)
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, 2f, closetTargets, layerMask);

            for (int i = 0; i < count; i++)
            {
                Collider closetTarget = closetTargets[i];

                if (closetTarget.gameObject.TryGetComponent(out Player player))
                {
                    player.IsAllowRiposte = true;
                    player.OnStartRiposte += () =>
                    {
                        animator.SetTrigger("DoRiposteVictim");
                    };

                    this.closetTarget = player.gameObject;
                }
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, 2f);
    }
}
