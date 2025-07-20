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

    Camera mainCamera;

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
        animator = GetComponent<Animator>();

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("No main camera found. Make sure your camera has the 'MainCamera' tag.");
        }

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
        float velocityAddition = isSprinting ? sprintAdittion : 0f;

        // Read input from Input System
        Vector3 input = new Vector3(inputHorizontal, 0f, inputVertical);

        // Fix control direction: rotate input 90° clockwise
        Vector3 rotatedInput = new Vector3(input.z, 0f, -input.x);

        // Apply speed and deltaTime
        Vector3 move = rotatedInput.normalized * (velocity + velocityAddition) * Time.fixedDeltaTime;

        // Handle vertical movement (jumping + gravity)
        float directionY = 0f;

        if (isJumping)
        {
            directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.3f, jumpElapsedTime / jumpTime) * Time.deltaTime;
            jumpElapsedTime += Time.deltaTime;

            if (jumpElapsedTime >= jumpTime)
            {
                isJumping = false;
                jumpElapsedTime = 0f;
            }
        }

        directionY -= gravity * Time.deltaTime;

        // Add vertical movement
        Vector3 movement = move + Vector3.up * directionY;

        // Move the character
        cc.Move(movement);

        // Rotate the character to face movement direction
        if (rotatedInput != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rotatedInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.15f);
        }
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
