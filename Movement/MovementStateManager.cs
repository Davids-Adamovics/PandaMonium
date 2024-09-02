using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    [SerializeField] public ParticleSystem dust;
    [SerializeField] public ParticleSystem dashDust;
    [SerializeField] public ParticleSystem jumpExplode;
    public float moveSpeed = 5;
    [HideInInspector] public Vector3 direction;
    public float hzInput, vInput;
    public CharacterController controller;
    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float gravity = -9.81f; // Earth's gravity: -9.81
    [SerializeField] float jumpForce = 8;
    [SerializeField] float dashForce = 100;
    [HideInInspector] public bool jumped;
    [HideInInspector] public bool dashed;
    public Vector3 velocity;
    Vector3 playerPosition;

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();
    public JumpState Jump = new JumpState();
    public DashState Dash = new DashState();
    public RunSlashState RunSlash = new RunSlashState();
    public WalkSlashState WalkSlash = new WalkSlashState();
    public StandSlashState StandSlash = new StandSlashState();

    public MovementBaseState previousState;
    public MovementBaseState currentState;
    private MovementBaseState nextState;

    [HideInInspector] public Animator animator;

    // Reference to the person GameObject
    [SerializeField] public GameObject person;

    // Mouse look sensitivity
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    // Camera reference
    [SerializeField] private Transform cameraTransform;

    // Limits for vertical camera movement
    [SerializeField] private float verticalLookLimitMin = -15f;
    [SerializeField] private float verticalLookLimitMax = 15f;

    // Rotation speed variable
    [SerializeField] private float rotationSpeed = 10f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found.");
        }

        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("CharacterController component not found.");
        }

        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform is not assigned.");
        }

        hzInput = 0;
        vInput = 0;

        Debug.Log("Initializing MovementStateManager. Setting initial state to Idle.");
        SwitchState(Idle);
    }


    void Update()
    {
        if (currentState == null)
        {
            Debug.LogError("CurrentState is not set.");
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Animator is not assigned.");
            return;
        }

        HandleCameraRotation();
        getDirectionAndMove();
        animator.SetFloat("hzInput", hzInput); // Ensure initial input is zero
        animator.SetFloat("vInput", vInput);

        currentState.UpdateState(this);
        ApplyGravity();

        if (nextState != null && nextState != currentState)
        {
            SwitchState(nextState);
            nextState = null;
        }
    }


    public void SwitchState(MovementBaseState state)
    {
        if (currentState != null)
        {
            currentState.ExitState(this, state);
        }

        currentState = state;
        currentState.EnterState(this);
        Debug.Log("Switched to state: " + state.GetType().Name);
    }

    public void RequestStateChange(MovementBaseState newState)
    {
        nextState = newState;
    }

    void getDirectionAndMove()
    {
        if (person == null)
        {
            Debug.LogError("Person GameObject is not assigned.");
            return;
        }

        hzInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");

        direction = transform.forward * vInput + transform.right * hzInput;

        Debug.Log($"Horizontal Input: {hzInput}, Vertical Input: {vInput}, Direction: {direction}");

        if (direction.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            if (currentState is RunState)
            {
                targetRotation *= Quaternion.Euler(0, 35f, 0);
            }
            else if (currentState is CrouchState)
            {
                targetRotation *= Quaternion.Euler(0, 35f, 0);
            }

            person.transform.rotation = Quaternion.Slerp(person.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        controller.Move(direction.normalized * moveSpeed * Time.deltaTime);
    }


    public void ApplyCrouchRotationOffset(float offset)
    {
        Quaternion rotationOffset = Quaternion.Euler(0, offset, 0);
        Quaternion targetRotation = Quaternion.LookRotation(direction) * rotationOffset;

        person.transform.rotation = Quaternion.Slerp(person.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    public bool isGrounded()
    {
        playerPosition = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        return Physics.CheckSphere(playerPosition, controller.radius - 0.05f, groundMask);
    }

    void ApplyGravity()
    {
        if (isGrounded() && velocity.y < 0) velocity.y = -2f;
        else velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (controller == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerPosition, controller.radius - 0.05f);
    }

    public void JumpForce() => velocity.y = jumpForce;
    public void Jumped() => jumped = true;

    public void DashForce() => moveSpeed = dashForce;
    public void Dashed() => dashed = true;

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, verticalLookLimitMin, verticalLookLimitMax);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
