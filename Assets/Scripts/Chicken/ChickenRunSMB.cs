using UnityEngine;

public class ChickenRunSMB : StateMachineBehaviour
{
    private ChickenController chicken;
    private float timer;
    private Vector2 fleeDirection;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (chicken == null) chicken = animator.GetComponent<ChickenController>();

        // Calcular dirección de huida del jugador
        Vector2 toPlayer = (chicken.player.position - chicken.transform.position).normalized;
        Vector2 fleeBase = -toPlayer; // Dirección opuesta al jugador
        
        // Añadir un poco de variación aleatoria para que no sea completamente lineal
        Vector2 randomOffset = Random.insideUnitCircle * 0.3f;
        fleeDirection = (fleeBase + randomOffset).normalized;
        
        // Aplicar velocidad de huida
        chicken.moveDir = fleeDirection * chicken.runSpeed;
        
        // Tiempo que correrá
        timer = Random.Range(1.5f, 2.5f);
        
        Debug.Log($"Entering {this.GetType().Name} - Flee direction: {fleeDirection}, Duration: {timer:F2}s");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer -= Time.deltaTime;
        
        // Si ya no está cerca el jugador O se acabó el tiempo, volver a idle
        if (timer <= 0f || !animator.GetBool("PlayerNear"))
        {
            // Limpiar la flag de jugador cerca
            animator.SetBool("PlayerNear", false);
            animator.SetTrigger("Idle");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Detener movimiento al salir del estado
        if (chicken != null)
        {
            chicken.moveDir = Vector2.zero;
        }
    }
}