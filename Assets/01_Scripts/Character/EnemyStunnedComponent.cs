using UnityEngine;

public class EnemyStunnedComponent : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    private Animator animator;
    private CharacterStateComponent stateComponent;

    private Collider[] allowRiposteColliders = new Collider[1];
    private GameObject allowRiposteObject;

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
                enabled = true;
            }
            else
            {
                enabled = false;
            }
        };

        enabled = false;
    }

    private void Update()
    {
        if (allowRiposteObject != null)
            return;

        int count = Physics.OverlapSphereNonAlloc(transform.position, 1.2f, allowRiposteColliders, layerMask);

        for (int i = 0; i < count; i++)
        {
            Collider collider = allowRiposteColliders[i];

            if (collider.gameObject.TryGetComponent(out Player player))
            {
                player.IsAllowRiposte = true;
                player.OnStartRiposte += OnStartRiposte;

                allowRiposteObject = player.gameObject;
            }
        }
    }

    private void OnEnable()
    {
        
    }

    private void OnStartRiposte()
    {
        allowRiposteObject = null;

        stateComponent.SetDamagedState();

        animator.SetTrigger("DoRiposteVictim");
    }
}
