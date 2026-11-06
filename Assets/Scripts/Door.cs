using UnityEngine;

public class Door : MonoBehaviour
{
    Animator animator;
    bool playerInside = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            animator.SetBool("isOpen", true);
            animator.SetBool("isStaying", false);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerInside)
        {
            animator.SetBool("isOpen", true);
            animator.SetBool("isStaying", true);
            animator.SetBool("isClosed", false);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            animator.SetBool("isOpen", false);
            animator.SetBool("isStaying", false);
            animator.SetBool("isClosed", true);
        }
    }
}

