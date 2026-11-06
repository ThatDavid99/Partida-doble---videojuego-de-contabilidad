using System.Collections;
using UnityEngine;

public class TransparentV : MonoBehaviour
{
    [Header("Sprites")]
    private SpriteRenderer spriteRenderer;

    [Header("Sorting")]
    [SerializeField] private Transform player;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
}