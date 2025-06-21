using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    public float velocity = 5f;
    public float sprintAdittion = 3.5f;
    public float jumpForce = 18f;
    public float jumpTime = 0.85f;
    public float gravity = 9.8f;

    float jumpElapsedTime = 0;
    bool isJumping = false;
    bool isSprinting = false;

    float inputHorizontal;
    float inputVertical;
    bool inputJump;
    bool inputSprint;

    Animator animator;
    CharacterController cc;

    PlayerInputActions inputActions;

    Camera scCamera;
    void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Jump.performed += ctx => inputJump = true;
        inputActions.Player.Run.performed += ctx => inputSprint = true;
        inputActions.Player.Run.canceled += ctx => inputSprint = false;
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    void Start()
    {
        cc = GetComponent<CharacterController>();
        scCamera = GetComponent<SideScrollerCamera>().cam;
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogWarning("Missing Animator component.");
    }

    void Update()
    {
        Vector2 moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        inputHorizontal = moveInput.x;
        inputVertical = moveInput.y;

        if (cc.isGrounded && animator != null)
        {
            animator.SetBool("run", cc.velocity.magnitude > 0.9f);
            isSprinting = cc.velocity.magnitude > 0.9f && inputSprint;
            animator.SetBool("sprint", isSprinting);
        }

        if (animator != null)
            animator.SetBool("air", !cc.isGrounded);

        if (inputJump && cc.isGrounded)
        {
            isJumping = true;
            inputJump = false; // reset trigger
        }

        HeadHittingDetect();
    }

    private void FixedUpdate()
    {
        float velocityAdittion = isSprinting ? sprintAdittion : 0;
        float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.fixedDeltaTime;
        float directionZ = inputVertical * (velocity + velocityAdittion) * Time.fixedDeltaTime;
        float directionY = 0;

        if (isJumping)
        {
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.3f, jumpElapsedTime / jumpTime) * Time.deltaTime;
            jumpElapsedTime += Time.deltaTime;
            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0;
            }
        }

        directionY -= gravity * Time.deltaTime;

        Vector3 forward = scCamera.transform.forward;
        Vector3 right = scCamera.transform.right;
        forward.y = right.y = 0;
        forward.Normalize();
        right.Normalize();

        forward *= directionZ;
        right *= directionX;

        if (directionX != 0 || directionZ != 0)
        {
            float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
        }

        Vector3 movement = (forward + right) + (Vector3.up * directionY);
        cc.Move(movement);
    }

    void HeadHittingDetect()
    {
        float headHitDistance = 1.1f;
        Vector3 ccCenter = transform.TransformPoint(cc.center);
        float hitCalc = cc.height / 2f * headHitDistance;

        if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
        {
            jumpElapsedTime = 0;
            isJumping = false;
        }
    }
}
