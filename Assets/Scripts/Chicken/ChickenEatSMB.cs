using UnityEngine;

public class ChickenEatSMB : StateMachineBehaviour
{
    private ChickenController chicken;
    private float timer;
    private float duration;
    private Rigidbody2D rb;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (chicken == null) chicken = animator.GetComponent<ChickenController>();
        if (rb == null) rb = animator.GetComponent<Rigidbody2D>();
        
        // Detener completamente el movimiento
        rb.linearVelocity = Vector2.zero;
        chicken.moveDir = Vector2.zero;
        
        // Duración aleatoria para comer
        duration = Random.Range(2f, 4f);
        timer = 0f;
        
        Debug.Log($"Entering {this.GetType().Name} - Duration: {duration:F2}s");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Si el jugador está cerca, interrumpir la comida y correr
        if (animator.GetBool("PlayerNear"))
        {
            animator.SetTrigger("Run");
            return;
        }
        
        timer += Time.deltaTime;
        
        // Cuando termine de comer, volver a idle
        if (timer >= duration)
        {
            animator.SetTrigger("Idle");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Asegurar que no hay movimiento al salir
        if (chicken != null)
        {
            chicken.moveDir = Vector2.zero;
        }
    }
}