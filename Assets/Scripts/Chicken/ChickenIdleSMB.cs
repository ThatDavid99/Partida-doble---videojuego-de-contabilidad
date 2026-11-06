using UnityEngine;

public class ChickenIdleSMB : StateMachineBehaviour
{
    private ChickenController chicken;
    private float timer;
    private float duration;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (chicken == null) chicken = animator.GetComponent<ChickenController>();
        
        // Detener movimiento completamente
        chicken.rb.linearVelocity = Vector2.zero;
        chicken.moveDir = Vector2.zero;
        
        // Duración para estar idle
        duration = Random.Range(2f, 4f);
        timer = 0f;
        
        Debug.Log($"[IDLE] Entrando - Duración: {duration:F2}s");
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        
        Debug.Log($"[IDLE] Timer: {timer:F2}/{duration:F2} - PlayerNear: {animator.GetBool("PlayerNear")}");
        
        // Si el jugador está cerca, salir a correr
        if (animator.GetBool("PlayerNear"))
        {
            Debug.Log("[IDLE] Jugador cerca - cambiando a Run");
            animator.SetTrigger("Run");
            return;
        }
        
        // Si terminó el tiempo, elegir siguiente acción
        if (timer >= duration)
        {
            float random = Random.value;
            Debug.Log($"[IDLE] Tiempo cumplido - Random value: {random}");
            
            if (random < 0.5f) // 50% probabilidad de comer
            {
                Debug.Log("[IDLE] Eligiendo EAT");
                animator.SetTrigger("Eat");
            }
            else
            {
                Debug.Log("[IDLE] Eligiendo WALK");
                animator.SetTrigger("Walk");
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("[IDLE] Saliendo del estado");
    }
}