using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public partial class Player : Character, IDamagable, IBlockable
{
    [SerializeField]
    private GameObject swordPrefab;

    [SerializeField]
    private GameObject shieldPrefab;

    [SerializeField]
    private float mouseSpeed = 0.25f;

    [SerializeField]
    private Vector2 pitchAngleLimit = new(20, 340);

    /**
     * Public Method
     */
    public Action<Player> OnDead;

    /**
     * Compoent
     */
    private Transform swordHolsterTransform;
    private Transform swordSlotTransform;
    private Transform shieldSlotTransform;
    private PlayerMoveComponent moveComponent;
    private HpComponent hpComponent;

    /**
     * Valuable
     */
    private Sword sword;
    private Quaternion rotation;
    private Transform cameraTargetTransform;
    private bool isShieldBlocking = false;
    private bool isJustGuarding = false;
    private bool isCounterAttackTiming = false;

    public bool IsAllowRiposte;
    public Action OnStartRiposte;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        moveComponent = GetComponent<PlayerMoveComponent>();
        hpComponent = GetComponent<HpComponent>();
    }

    private void Start()
    {
        cameraTargetTransform = transform.FindChildByName("CameraTarget");
        swordHolsterTransform = transform.FindChildByName("Holster_Sword");
        swordSlotTransform = transform.FindChildByName("WeaponSlot_Sword");
        shieldSlotTransform = transform.FindChildByName("WeaponSlot_Shield");

        if (swordPrefab != null)
        {
            GameObject instance = Instantiate(swordPrefab, swordHolsterTransform);
            sword = instance.GetComponent<Sword>();
        }

        if (shieldPrefab != null)
        {
            GameObject go = Instantiate(shieldPrefab, shieldSlotTransform);
        }

        StaggerFrameManager.Instance.AddAnimator(gameObject.GetInstanceID(), animator);
        GameRuleManager.Instance.RegisterPlayer(this);

        LockCursor();        
    }

    private void Update()
    {
        Update_CamearTarget();
        Update_KeyInputEquip();
        Update_KeyInputAttack();
        Update_KeyInputShieldBlock();
    }

    private void Update_CamearTarget()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotation *= Quaternion.AngleAxis(mouseX, Vector3.up);
        rotation *= Quaternion.AngleAxis(-mouseY, Vector3.right);

        cameraTargetTransform.rotation = rotation;

        Vector3 angle = cameraTargetTransform.localEulerAngles;
        angle.z = 0f;

        float xAngle = cameraTargetTransform.localEulerAngles.x;

        if (xAngle < 180f && xAngle > pitchAngleLimit.x)
            angle.x = pitchAngleLimit.x;
        else if (xAngle > 180f && xAngle < pitchAngleLimit.y)
            angle.x = pitchAngleLimit.y;

        cameraTargetTransform.localEulerAngles = angle;

        rotation = Quaternion.Lerp(cameraTargetTransform.rotation, rotation, mouseSpeed * Time.deltaTime);

        cameraTargetTransform.localEulerAngles = new(angle.x, 0, 0);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }

    public void Damage(GameObject attacker, Sword causer, Vector3 hitPoint, WeaponActionData actionData, Action<DamageResult> callback)
    {
        ResetFlag();

        if (isJustGuarding)
        {
            if (actionData.Particle != null)
            {
                GameObject go = Instantiate(actionData.Particle, transform, false);
                go.transform.localPosition = hitPoint + actionData.ParticleOffset;
                go.transform.localScale = actionData.ParticleScale;
            }

            StartCoroutine(Coroutine_CounterAttackTiming());

            callback?.Invoke(DamageResult.JustBlocked);

            return;
        }

        if (isShieldBlocking)
        {
            animator.SetTrigger("DoShieldBlockImpact");

            callback?.Invoke(DamageResult.Blocked);

            return;
        }

        hpComponent.Damage(actionData.Power);

        callback?.Invoke(DamageResult.Success);

        if (hpComponent.IsDead == false)
        {
            animator.SetTrigger("DoHit");
        }
        else
        {
            animator.SetTrigger("DoDeath");
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            gameObject.GetComponent<Collider>().enabled = false;

            OnDead?.Invoke(this);
        }
    }

    public void CriticalDamage(GameObject attacker, Vector3 hitPoint, WeaponActionData actionData, Action<DamageResult> callback)
    {

    }

    public bool IsBlocked(GameObject attacker)
    {
        return isShieldBlocking;
    }

    private IEnumerator Coroutine_CounterAttackTiming()
    {
        isCounterAttackTiming = true;

        yield return new WaitForSeconds(1f);

        isCounterAttackTiming = false;
    }

    private void LockCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void UnlockCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
