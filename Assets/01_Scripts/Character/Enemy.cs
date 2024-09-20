using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HpComponent))]
public partial class Enemy : Character, IDamagable
{
    [SerializeField]
    private WeaponActionData attackAactionData;

    [SerializeField]
    private GameObject swordPrefab;

    [SerializeField]
    private GameObject shieldPrefab;

    #region Public Valuable
    public Action<Enemy> OnDead;
    #endregion

    #region Component
    private HpComponent hpComponent;
    private EnemyMoveComponent moveComponent;
    private EnemyScanComponent scanComponent;
    private EnemyPatrolComponent patrolComponent;
    #endregion

    #region Valuable
    private List<Material> materialList;
    private List<Color> originColorList;
    private Transform swordSlotTransform;
    private Transform shieldSlotTransform;
    private Sword_Enemy sword;
    #endregion

    #region Coroutine
    private Coroutine chasePlayerCoroutine;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        hpComponent = GetComponent<HpComponent>();
        moveComponent = GetComponent<EnemyMoveComponent>();
        patrolComponent = GetComponent<EnemyPatrolComponent>();
        
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
        swordSlotTransform = transform.FindChildByName("WeaponSlot_Sword");
        shieldSlotTransform = transform.FindChildByName("WeaponSlot_Shield");

        if (swordPrefab != null)
        {
            GameObject go = Instantiate(swordPrefab, swordSlotTransform);
            sword = go.GetComponent<Sword_Enemy>();
        }

        if (shieldPrefab != null)
        {
            GameObject go = Instantiate(shieldPrefab, shieldSlotTransform);
        }

        StaggerFrameManager.Instance.AddAnimator(gameObject.GetInstanceID(), animator);
        GameRuleManager.Instance.RegisterEnemy(this);

        patrolComponent.StartPatrol();

        Block();
    }

    public void Damage(GameObject attacker, Sword causer, Vector3 hitPoint, WeaponActionData actionData)
    {
        patrolComponent.StopPatrol();

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

            OnDead?.Invoke(this);
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

    private void OnFoundPlayer(Player player)
    {
        Debug.Log("OnFoundPlayer");

        patrolComponent.StopPatrol();

        chasePlayerCoroutine = StartCoroutine(Coroutine_ChasePlayer(player));
    }

    private void OnLostPlayer()
    {
        Debug.Log("OnLostPlayer");

        StopCoroutine(chasePlayerCoroutine);

        patrolComponent.StartPatrol(3f);

        moveComponent.StopMove();
    }

    private IEnumerator Coroutine_ChasePlayer(Player player)
    {
        WaitForSeconds wait = new WaitForSeconds(2f);

        while (true)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= scanComponent.AttackRange)
            {
                moveComponent.StopMove();

                //Attack();

                yield return wait;
            }
            else
            {
                moveComponent.SetMove(player.transform.position, 2f);
            }

            yield return null;
        }
    }

    private void Attack()
    {
        isComboExist = true;

        animator.SetBool("Attack", true);
    }

    private void Block()
    {
        animator.SetBool("Block", true);
    }

    private void OnDrawGizmosSelected()
    {
        //if (Application.isPlaying == false)
        //    return;

        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(destination, 0.25f);
    }
}
