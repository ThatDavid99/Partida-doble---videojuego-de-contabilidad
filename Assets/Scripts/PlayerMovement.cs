using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;

    private Vector2 movement;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private SimpleTreeChopper treeChopper;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        treeChopper = GetComponent<SimpleTreeChopper>();
    }
    void Update()
    {
        if (treeChopper != null && treeChopper.isChopping)
        {
            animator.SetFloat("Speed", 0); // Detener animación de correr
            return; // No actualizar movimiento
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement = movement.normalized;

        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", rb.linearVelocity.magnitude);
    }
    void FixedUpdate()
    {
        rb.linearVelocity = Vector2.ClampMagnitude(movement * moveSpeed, moveSpeed);
    }
}
