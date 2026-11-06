using UnityEngine;

public class ChickenWalkSMB : StateMachineBehaviour
{
    private ChickenController chicken;
    private float timer;
    private float duration;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (chicken == null) chicken = animator.GetComponent<ChickenController>();
        
        duration = Random.Range(1f, 3f);
        timer = 0f;
        
        // MÉTODO ALTERNATIVO para dirección aleatoria
        float angle = Random.Range(0f, 360f);
        Vector2 direction = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad), 
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );
        
        chicken.moveDir = direction * chicken.walkSpeed;
        
        Debug.Log($"[WALK] Entrando - Ángulo: {angle:F1}°, Dirección: {direction}, Duración: {duration:F2}s");
        Debug.Log($"[WALK] MoveDir aplicado: {chicken.moveDir}");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        
        // Debug cada segundo
        if (Mathf.FloorToInt(timer) != Mathf.FloorToInt(timer - Time.deltaTime))
        {
            Debug.Log($"[WALK] Timer: {timer:F2}/{duration:F2} - Velocity: {chicken.rb.linearVelocity} - PlayerNear: {animator.GetBool("PlayerNear")}");
        }
        
        // Si jugador cerca, correr
        if (animator.GetBool("PlayerNear"))
        {
            Debug.Log("[WALK] Jugador cerca - cambiando a Run");
            animator.SetTrigger("Run");
            return;
        }
        
        // Si terminó el tiempo, volver a idle
        if (timer >= duration)
        {
            Debug.Log("[WALK] Tiempo cumplido - volviendo a Idle");
            animator.SetTrigger("Idle");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("[WALK] Saliendo - deteniendo movimiento");
        if (chicken != null)
        {
            chicken.moveDir = Vector2.zero;
        }
    }
}