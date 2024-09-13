using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HpComponent))]
public class Enemy : Character, IDamagable
{
    #region Component
    private HpComponent hpComponent;
    private EnemyMoveState moveState;
    private EnemyScanComponent scanComponent;
    #endregion

    #region Valuable
    private List<Material> materialList;
    private List<Color> originColorList;
    #endregion

    #region Coroutine
    private Coroutine patrolCoroutine;
    private Coroutine chasePlayerCoroutine;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        hpComponent = GetComponent<HpComponent>();
        moveState = GetComponent<EnemyMoveState>();
        
        scanComponent = GetComponent<EnemyScanComponent>();
        {
            scanComponent.OnFoundPlayerAction += OnFoundPlayer;
            scanComponent.OnLostPlayerAction += OnLostPlayer;
        }

        materialList = new();
        originColorList = new();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                materialList.Add(material);
                originColorList.Add(material.color);
            }
        }
    }

    private void Start()
    {
        StaggerFrameManager.Instance.AddAnimator(gameObject.GetInstanceID(), animator);

        patrolCoroutine = StartCoroutine(Coroutine_Patrol(0f));
        //moveState.Destination = GetRandomPosition();
        //moveState.MoveSpeed = 1f;
    }

    public void Damage(GameObject attacker, Sword causer, Vector3 hitPoint, WeaponActionData actionData)
    {
        StopCoroutine(patrolCoroutine);
        animator.SetFloat("SpeedZ", 0f);

        hpComponent.Damage(actionData.Power);
        
        foreach (Material material in materialList)
        {
            material.color = Color.red;
        }

        StartCoroutine(RestoreColor(0.15f));

        StaggerFrameManager.Instance.Delay(actionData.StaggerFrame);

        if (actionData.Particle != null)
        {
            GameObject go = Instantiate(actionData.Particle, transform, false);
            go.transform.localPosition = hitPoint + actionData.ParticleOffset;
            go.transform.localScale = actionData.ParticleScale;
        }

        if (hpComponent.IsDead)
        {
            animator.SetTrigger("DoDeath");
            gameObject.GetComponent<Collider>().enabled = false;
        }
        else
        {
            transform.LookAt(attacker.transform, Vector3.up);

            animator.SetTrigger("DoHit");

            rigidbody.isKinematic = false;

            float launch = rigidbody.drag * actionData.KnockbackDist * 10f;
            rigidbody.AddForce(-transform.forward * launch);

            StartCoroutine(Coroutine_RestoreIsKinemetics(5));
        }
    }

    private IEnumerator Coroutine_RestoreIsKinemetics(int frame)
    {
        WaitForFixedUpdate wait = new();

        for (int i = 0; i < frame; i++)
        {
            yield return wait;
        }

        rigidbody.isKinematic = true;
    }

    private IEnumerator RestoreColor(float delay)
    {
        yield return new WaitForSeconds(delay);

        for (int i = 0; i < materialList.Count; i++)
        {
            materialList[i].color = originColorList[i];
        }
    }

    private IEnumerator Coroutine_Patrol(float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        WaitForFixedUpdate wait = new();
        
        bool isFirst = true;

        moveState.Destination = transform.position;

        while (true)
        {
            if (Vector3.Distance(moveState.Destination, transform.position) < 0.1f)
            {
                float waitTime = UnityEngine.Random.Range(1f, 3f);

                if (isFirst)
                {
                    isFirst = false;
                    waitTime = 0f;
                }

                yield return new WaitForSeconds(waitTime);

                moveState.Destination = GetRandomPosition();
                moveState.MoveSpeed = 1f;
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

    private void OnFoundPlayer(Player player)
    {
        Debug.Log("OnFoundPlayer");

        StopCoroutine(patrolCoroutine);

        chasePlayerCoroutine = StartCoroutine(Coroutine_ChasePlayer(player));
    }

    private void OnLostPlayer()
    {
        Debug.Log("OnLostPlayer");

        StopCoroutine(chasePlayerCoroutine);

        patrolCoroutine = StartCoroutine(Coroutine_Patrol(3f));

        moveState.Destination = transform.position;
        moveState.MoveSpeed = 0f;
    }

    private IEnumerator Coroutine_ChasePlayer(Player player)
    {
        WaitForSeconds wait = new WaitForSeconds(2f);

        while (true)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= scanComponent.AttackRange)
            {
                moveState.Destination = transform.position;
                moveState.MoveSpeed = 0f;

                animator.SetBool("Attack", true);

                yield return wait;
            }
            else
            {
                moveState.Destination = player.transform.position;
                moveState.MoveSpeed = 2f;
            }

            yield return null;
        }
    }

    private void End_Attack()
    {
        animator.SetBool("Attack", false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return;
        }

        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            player.OnDamaged(5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        //if (Application.isPlaying == false)
        //    return;

        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(destination, 0.25f);
    }
}
