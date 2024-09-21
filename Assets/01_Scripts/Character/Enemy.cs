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
    private CharacterStateComponent stateComponent;
    private EnemyMoveComponent moveComponent;
    private EnemyScanComponent scanComponent;
    private EnemyPatrolComponent patrolComponent;
    private EnemyChaseComponent chaseComponent;
    private EnemyDamagedComponent damagedComponent;
    #endregion

    #region Valuable
    private List<Material> materialList;
    private List<Color> originColorList;
    private Transform swordSlotTransform;
    private Transform shieldSlotTransform;
    private Sword_Enemy sword;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        stateComponent = GetComponent<CharacterStateComponent>();
        hpComponent = GetComponent<HpComponent>();
        moveComponent = GetComponent<EnemyMoveComponent>();
        patrolComponent = GetComponent<EnemyPatrolComponent>();
        chaseComponent = GetComponent<EnemyChaseComponent>();
        damagedComponent = GetComponent<EnemyDamagedComponent>();
        
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
            
            sword.DamageResultCallback += (result) =>
            {
                switch (result)
                {
                    case DamageResult.Success:
                    {
                        OnAttackSuccess();
                    }
                    break;

                    case DamageResult.Blocked:
                    {
                        OnAttackBlocked();
                    }
                    break;

                    case DamageResult.JustBlocked:
                    {
                        OnAttackJustBlocked();
                    }
                    break;
                }
            };
        }

        if (shieldPrefab != null)
        {
            GameObject go = Instantiate(shieldPrefab, shieldSlotTransform);
        }

        StaggerFrameManager.Instance.AddAnimator(gameObject.GetInstanceID(), animator);
        GameRuleManager.Instance.RegisterEnemy(this);

        patrolComponent.StartPatrol();
    }

    private void OnAttackSuccess()
    {
        Debug.Log("AttackSuccess");
    }

    private void OnAttackBlocked()
    {
        Debug.Log("AttackBlocked");
    }

    private void OnAttackJustBlocked()
    {
        Debug.Log("AttackJustBlocked");

        animator.SetTrigger("DoHit");

        //stateComponent.SetSturnedState();
    }

    public void Damage(GameObject attacker, Sword causer, Vector3 hitPoint, WeaponActionData actionData, Action<DamageResult> callback)
    {
        patrolComponent.StopPatrol();

        DamageResult result = damagedComponent.TryDamage();

        if (result == DamageResult.Success)
        {
            hpComponent.Damage(actionData.Power);
        }
        else
        {
            animator.SetTrigger("DoShieldBlockImpact");
            return;
        }

        animator.SetFloat("SpeedZ", 0f);

        SetDamagedColor();

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

    public void CriticalDamage(GameObject attacker, Vector3 hitPoint, WeaponActionData actionData, Action<DamageResult> callback)
    {
        patrolComponent.StopPatrol();

        SetDamagedColor();

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

            animator.SetTrigger("DoStunned");
            stateComponent.SetStunnedState();

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

    private void SetDamagedColor()
    {
        foreach (Material material in materialList)
        {
            material.color = Color.red;
        }

        StartCoroutine(RestoreColor(0.15f));
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

        chaseComponent.StartChase(player.gameObject);
    }

    private void OnLostPlayer()
    {
        Debug.Log("OnLostPlayer");

        chaseComponent.StopChase();

        patrolComponent.StartPatrol(3f);

        moveComponent.StopMove();
    }

    private void OnDrawGizmosSelected()
    {
        //if (Application.isPlaying == false)
        //    return;

        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(destination, 0.25f);
    }
}
