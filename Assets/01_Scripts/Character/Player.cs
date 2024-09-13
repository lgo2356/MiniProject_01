using UnityEngine;

[RequireComponent(typeof(Animator))]
public partial class Player : Character
{
    [SerializeField]
    private GameObject swordPrefab;

    [SerializeField]
    private float mouseSpeed = 0.25f;

    [SerializeField]
    private Vector2 limitPitchAngle = new(20, 340);

    /**
     * Compoent
     */
    private Transform swordHolsterTransform;
    private Transform swordSlotTransform;
    private PlayerMoveState moveState;

    /**
     * Valuable
     */
    private Sword sword;
    private Quaternion rotation;
    private Transform cameraTargetTransform;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        moveState = GetComponent<PlayerMoveState>();
    }

    private void Start()
    {
        cameraTargetTransform = transform.FindChildByName("CameraTarget");

        swordHolsterTransform = transform.FindChildByName("Holster_Sword");
        swordSlotTransform = transform.FindChildByName("WeaponSlot_Sword");

        if (swordPrefab != null)
        {
            GameObject instance = Instantiate(swordPrefab, swordHolsterTransform);
            sword = instance.GetComponent<Sword>();
        }

        StaggerFrameManager.Instance.AddAnimator(gameObject.GetInstanceID(), animator);

        LockCursor();        
    }

    private void Update()
    {
        Update_CamearTarget();
        Update_InputEquip();
        Update_InputAttack();
    }

    public void OnDamaged(float amount)
    {
        animator.SetBool("Hit", true);
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

        if (xAngle < 180f && xAngle > limitPitchAngle.x)
            angle.x = limitPitchAngle.x;
        else if (xAngle > 180f && xAngle < limitPitchAngle.y)
            angle.x = limitPitchAngle.y;

        cameraTargetTransform.localEulerAngles = angle;

        rotation = Quaternion.Lerp(cameraTargetTransform.rotation, rotation, mouseSpeed * Time.deltaTime);

        cameraTargetTransform.localEulerAngles = new(angle.x, 0, 0);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }

    private void End_Hit()
    {
        animator.SetBool("Hit", false);
    }
}
