using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Required for InputAction

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float interactRadius = 2f;

    private Rigidbody2D rb;

    // Input Actions
    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction interactAction;

    [SerializeField]
    private bool inputDisabled = false;

    private Interactable currentTarget;
    public string playerName;
    private Vector2 moveInput;

    private readonly List<Interactable> nearbyInteractables = new();
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
    private void Update()
    {
        // Only update target if player is moving noticeably
        if (rb.linearVelocity.sqrMagnitude > 0)
        {
            UpdateInteractableTarget();
        }
    }// Called when trigger detects overlap
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.isTrigger) return;

        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable && !nearbyInteractables.Contains(interactable))
            nearbyInteractables.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Interactable interactable = other.GetComponent<Interactable>();
        if (interactable && nearbyInteractables.Contains(interactable))
        {
            nearbyInteractables.Remove(interactable);
            interactable.SetHighlight(false);
            if (interactable == currentTarget)
            {
                currentTarget = null;
            }
        }
    }

    private void UpdateInteractableTarget()
    {
        if (nearbyInteractables.Count == 0)
        {
            if (currentTarget != null)
            {
                currentTarget.SetHighlight(false);
                currentTarget = null;
            }
            return;
        }
        Interactable closest = null;
        float closestDist = float.MaxValue;

        foreach (var interactable in nearbyInteractables)
        {
            if (interactable == null) continue;
            float dist = Vector2.Distance(transform.position, interactable.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }

        // update highlight state
        if (closest != currentTarget)
        {
            if (currentTarget != null)
                currentTarget.SetHighlight(false);

            currentTarget = closest;

            if (currentTarget != null)
                currentTarget.SetHighlight(true);
        }
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        moveInput = moveAction.ReadValue<Vector2>().normalized;
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnAttack()
    {
        // TODO: Add attack animation / hitbox
    }

    private void OnInteract()
    {
        Debug.Log("interact");
        if (currentTarget != null)
            currentTarget.Interact();
    }
}
