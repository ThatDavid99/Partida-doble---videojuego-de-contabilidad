using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class ChickenController : MonoBehaviour
{
    [Header("Velocidades")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 4f;

    [Header("Detección de jugador")]
    public float detectionRadius = 5f;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    public bool showCurrentState = true;

    [HideInInspector] public Transform player;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Vector2 moveDir;
    
    private bool wasPlayerNear = false;
    private string currentState = "";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        
        // Buscar jugador más robustamente
        GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
        if (playerGO == null)
        {
            playerGO = GameObject.Find("Player"); // Fallback por nombre
        }
        
        if (playerGO != null)
        {
            player = playerGO.transform;
            if (showDebugLogs) Debug.Log($"[ChickenController] Player found: {player.name}");
        }
        else
        {
            Debug.LogError("[ChickenController] No se encontró el Player! Verifica el tag 'Player' o el nombre 'Player'");
        }
    }

    void Start()
    {
        if (showDebugLogs)
        {
            Debug.Log($"[ChickenController] Iniciado - WalkSpeed: {walkSpeed}, RunSpeed: {runSpeed}, DetectionRadius: {detectionRadius}");
            
            // Verificar parámetros del animator
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                Debug.Log($"[ChickenController] Parámetro del Animator: {param.name} ({param.type})");
            }
        }
    }

    void Update()
    {
        // Debug del estado actual
        if (showCurrentState)
        {
            string newState = anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") ? "Idle" :
                            anim.GetCurrentAnimatorStateInfo(0).IsName("Walk") ? "Walk" :
                            anim.GetCurrentAnimatorStateInfo(0).IsName("Eat") ? "Eat" :
                            anim.GetCurrentAnimatorStateInfo(0).IsName("Run") ? "Run" :
                            anim.GetCurrentAnimatorStateInfo(0).IsName("ChickenEating") ? "ChickenEating" :
                            anim.GetCurrentAnimatorStateInfo(0).IsName("ChickenIdle") ? "ChickenIdle" :
                            anim.GetCurrentAnimatorStateInfo(0).IsName("ChickenWalking") ? "ChickenWalking" :
                            anim.GetCurrentAnimatorStateInfo(0).IsName("ChickenRunning") ? "ChickenRunning" :
                            "Unknown: " + anim.GetCurrentAnimatorStateInfo(0).shortNameHash;
            
            if (newState != currentState)
            {
                currentState = newState;
                Debug.Log($"[ChickenController] Estado cambiado a: {currentState}");
            }
        }

        if (player != null)
        {
            float dist = Vector2.Distance(transform.position, player.position);
            bool isPlayerNear = dist < detectionRadius;
            
            if (showDebugLogs && isPlayerNear != wasPlayerNear)
            {
                Debug.Log($"[ChickenController] Proximidad del jugador cambió: {isPlayerNear} (Distancia: {dist:F2})");
            }
            
            if (isPlayerNear != wasPlayerNear)
            {
                anim.SetBool("PlayerNear", isPlayerNear);
                wasPlayerNear = isPlayerNear;
                
                if (isPlayerNear && showDebugLogs)
                {
                    Debug.Log("[ChickenController] ¡Jugador detectado! Intentando activar Run...");
                    // Intentar diferentes formas de activar el estado de correr
                    anim.SetTrigger("Run");
                }
            }
        }
        
        // Debug de movimiento
        if (showDebugLogs && moveDir != Vector2.zero)
        {
            Debug.Log($"[ChickenController] MoveDir: {moveDir}, Velocity: {rb.linearVelocity}");
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDir;
    }
    
    void OnDrawGizmos()
    {
        // Dibujar el radio de detección
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        
        // Dibujar dirección de movimiento
        if (moveDir != Vector2.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)moveDir);
        }
        
        // Dibujar línea al jugador si existe
        if (player != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }

    // Método para forzar transiciones desde el inspector (para debug)
    [ContextMenu("Force Idle")]
    public void ForceIdle() { anim.SetTrigger("Idle"); }
    
    [ContextMenu("Force Walk")]
    public void ForceWalk() { anim.SetTrigger("Walk"); }
    
    [ContextMenu("Force Eat")]
    public void ForceEat() { anim.SetTrigger("Eat"); }
    
    [ContextMenu("Force Run")]
    public void ForceRun() { anim.SetTrigger("Run"); }
}