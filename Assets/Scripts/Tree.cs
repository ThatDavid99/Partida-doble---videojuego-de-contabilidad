using System.Collections;
using UnityEngine;

public class SimpleTree : MonoBehaviour
{
    [Header("Tree Stats")]
    public int Health = 2;
    public int Damage = 0;

    [Header("Sprites")]
    [SerializeField] private Sprite[] treeSprites;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    [Header("Sorting")]
    [SerializeField] private Transform player;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
        RefreshSprite();
    }

    void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            
            if (distance < 1.3f)
            {
                spriteRenderer.sortingOrder = player.position.y-0.2 > transform.position.y ? 1 : -1;
                
                if (spriteRenderer.sortingOrder == 1)
                {
                    Color color = spriteRenderer.color;
                    color.a = 0.7f;
                    spriteRenderer.color = color;
                }
                else
                {
                    Color color = spriteRenderer.color;
                    color.a = 1f;
                    spriteRenderer.color = color;
                }
            }
            else
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
        }
    }

    public void GetHit()
    {
        Damage += 1;
        RefreshSprite();

        if (Damage >= Health)
        {
            TalarArbol();
        }
    }

    private void RefreshSprite()
    {
        if (treeSprites == null || treeSprites.Length == 0 || spriteRenderer == null) return;
        int spriteIndex = Mathf.Min(Damage, treeSprites.Length - 1);
        spriteRenderer.sprite = treeSprites[spriteIndex];
    }

    private void TalarArbol()
    {
        animator.enabled = true;
        animator.SetTrigger("Falling");
        StartCoroutine(WaitWhileanimation());
    }
    
    IEnumerator WaitWhileanimation()
    {
        yield return new WaitForSeconds(0.45f);
        Destroy(gameObject);
    }
}