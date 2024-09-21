using System;
using UnityEditor;
using UnityEngine;

public class EnemyScanComponent : MonoBehaviour
{
    [SerializeField]
    private float scanRange;

    [SerializeField]
    private float attackRange;

    [SerializeField]
    private LayerMask layerMask;

    private Collider[] scanTargets = new Collider[1];
    private Player scanPlayer;

    public Player ScanPlayer => scanPlayer;
    public Action<Player> OnFoundPlayerAction;
    public Action OnLostPlayerAction;

    public float AttackRange => 1.5f;

    private void Update()
    {
        Update_Scan();
    }

    private void Update_Scan()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, scanRange, scanTargets, layerMask);

        if (scanPlayer != null && count <= 0)
        {
            scanPlayer = null;

            OnLostPlayerAction?.Invoke();

            return;
        }

        for (int i = 0; i < count; i++)
        {
            Collider scanTarget = scanTargets[i];

            if (scanTarget.gameObject.TryGetComponent(out Player player))
            {
                Debug.DrawRay(transform.position, scanTarget.transform.position - transform.position, Color.red, 0.1f);

                if (scanPlayer == null)
                {
                    OnFoundPlayerAction?.Invoke(player);

                    scanPlayer = player;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, scanRange);

        Handles.color = Color.red;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, attackRange);
    }
}
