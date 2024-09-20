using UnityEngine;

public class PlayerMoveComponent : MonoBehaviour
{
    [SerializeField]
    private string cameraTargetName = "CameraTarget";

    [SerializeField]
    private float walkSpeed = 1.0f;

    [SerializeField]
    private float runSpeed = 3.0f;

    /**
     * Component
     */
    private Animator animator;

    /**
     * Variable
     */
    private Transform cameraTarget;
    private Vector3 direction;

    /**
     * Boolean
     */
    //private bool canMove = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cameraTarget = transform.FindChildByName(cameraTargetName);
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        bool isRun = Input.GetButton("Run");
        float speed = isRun ? runSpeed : walkSpeed;

        // 플레이어 이동 방향 계산
        direction = (Vector3.forward * vertical) + (Vector3.right * horizontal);
        direction = direction.normalized * speed;
        transform.Translate(direction * Time.deltaTime);

        // 애니메이션
        animator.SetFloat("SpeedX", horizontal * speed);
        animator.SetFloat("SpeedZ", vertical * speed);
    }
}
