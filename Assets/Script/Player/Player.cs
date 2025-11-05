using System;
using UnityEngine;
using UnityEngine.InputSystem; // Required for InputAction

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;

    // Input Actions
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction interactAction;

    [SerializeField]
    private bool inputDisabled = false;
    private Vector2 moveInput;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;

        moveAction = InputSystem.actions.FindAction("Player/Move");
        attackAction = InputSystem.actions.FindAction("Player/Attack");
        interactAction = InputSystem.actions.FindAction("Player/Interact");

        attackAction.performed += _ => OnAttack();
        interactAction.performed += _ => OnInteract();

        DisabledInput(inputDisabled);
    }
    public void DisabledInput(bool inputDisabled)
    {
        this.inputDisabled = inputDisabled;
        if (!inputDisabled)
        {
            moveAction?.Enable();
            attackAction?.Enable();
            interactAction?.Enable();
        }
        else
        {
            moveAction?.Disable();
            attackAction?.Disable();
            interactAction?.Disable();
        }
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        moveInput = moveAction.ReadValue<Vector2>().normalized;
        Debug.Log(moveInput);
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnAttack()
    {
        Debug.Log("Player attacked!");
        // TODO: Add attack animation / hitbox
    }

    private void OnInteract()
    {
        Debug.Log("Player interacted!");
        // TODO: Add interaction logic
    }
}
