using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SimpleTreeChopper : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private LayerMask treeLayer;
    private int lastHitFrame = -1;
    private Animator animator;
    private SimpleTree actualTree;
    private Rigidbody2D rb;
    public bool isChopping = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isChopping)
        {
            TryChop();
        }
    }
    private void TryChop()
    {
        animator.SetFloat("Speed", 0);
        if (actualTree != null && actualTree.Health != 0)
        {
            Vector2 dir = (actualTree.transform.position - transform.position).normalized;
            animator.SetFloat("ChopX", dir.x);
            animator.SetFloat("ChopY", dir.y);
            animator.SetBool("isChopping", true);
            StartCoroutine(WaitAndEnableMovement());
        }
        else
        {
            Collider2D treeCol = Physics2D.OverlapCircle(transform.position, attackRange, treeLayer);
            if (treeCol != null)
            {
                actualTree = treeCol.GetComponent<SimpleTree>();
                if (actualTree != null && actualTree.Health != 0)
                {
                    Vector2 dir = (actualTree.transform.position - transform.position).normalized;
                    animator.SetFloat("ChopX", dir.x);
                    animator.SetFloat("ChopY", dir.y);
                    animator.SetBool("isChopping", true);
                    StartCoroutine(WaitAndEnableMovement());
                }
            }
        }
    }
    
    public void golpearArbol()
    {
        if (Time.frameCount == lastHitFrame)
        {
            return;
        }
        lastHitFrame = Time.frameCount;
        if (actualTree != null)
        {
            actualTree.GetHit();
        }
        animator.SetBool("isChopping", false);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Trees"))
        {
            actualTree = other.GetComponent<SimpleTree>();
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Trees") && other.GetComponent<SimpleTree>() == actualTree)
        {
            actualTree = null;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    
    IEnumerator WaitAndEnableMovement()
    {
        isChopping = true;
        PlayerMovement.instance.enabled = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        yield return new WaitForSeconds(1.3f);
        PlayerMovement.instance.enabled = true;
        isChopping = false;
    }
}