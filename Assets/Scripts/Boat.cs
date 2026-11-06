using UnityEngine;

public class Boat : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationDeadzone = 0.1f;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionDistance = 2f;
    [SerializeField] private Vector3 playerOffset = new Vector3(-0.05f, 0.2f, 0);
    [SerializeField] private GameObject enterButton;
    
    [Header("Pier Settings")]
    [SerializeField] private float pierDetectionRadius = 2f;

    
    private Transform player;
    private Rigidbody2D boatRb;
    private Animator animator;
    private bool isInBoat = false;
    private bool facingLeft = true;
    private Quaternion enterButtonInitialRotation;
    private Vector3 enterButtonInitialLocalPosition;
    private GameObject nearestPierToPlayer;

    void Start()
    {
        boatRb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        facingLeft = Mathf.Approximately(transform.eulerAngles.y, 180f);
        if (enterButton != null)
        {
            enterButtonInitialRotation = enterButton.transform.rotation;
            enterButtonInitialLocalPosition = enterButton.transform.localPosition;
        }
    }

    void Update()
    {
        UpdateNearestPierToPlayer();
        HandleBoatInteraction();
        UpdateEnterButtonVisibility();

        if (isInBoat)
        {
            UpdatePlayerPosition();
            HandleBoatMovement();
        }
    }

    private void UpdateNearestPierToPlayer()
    {
        GameObject[] piers = GameObject.FindGameObjectsWithTag("Pier");
        nearestPierToPlayer = null;
        float closestDistanceToPlayer = pierDetectionRadius;

        foreach (GameObject pier in piers)
        {
            float distanceToPlayer = Vector2.Distance(player.position, pier.transform.position);
            if (distanceToPlayer < closestDistanceToPlayer)
            {
                closestDistanceToPlayer = distanceToPlayer;
                nearestPierToPlayer = pier;
            }
        }
    }

    private GameObject GetPierBoatIsOn()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);
        
        foreach (Collider2D col in colliders)
        {
            if (col.CompareTag("Pier"))
            {
                return col.gameObject;
            }
        }
        
        return null;
    }

    private void HandleBoatInteraction()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;
        
        float distance = Vector2.Distance(transform.position, player.position);
        GameObject pierBoatIsOn = GetPierBoatIsOn(); 

        if (!isInBoat && distance < interactionDistance && nearestPierToPlayer != null)
        {
            isInBoat = true;
            player.position = transform.position;
            player.SetParent(transform);
            PlayerMovement.instance.enabled = false;
            if (animator != null) animator.SetBool("isInBoat", true);
        }
        else if (isInBoat && pierBoatIsOn != null)
        {
            isInBoat = false;
            player.SetParent(null);
            
            Transform spawnPoint = pierBoatIsOn.transform.Find("SpawnPoint");
            if (spawnPoint != null)
            {
                player.position = spawnPoint.position;
            }
            else
            {
                player.position = pierBoatIsOn.transform.position;
            }
            
            PlayerMovement.instance.enabled = true;
            if (boatRb != null) boatRb.linearVelocity = Vector2.zero;
            if (animator != null) animator.SetBool("isInBoat", false);
        }
    }

    private void UpdateEnterButtonVisibility()
    {
        if (enterButton == null || player == null) return;
        float distance = Vector2.Distance(transform.position, player.position);
        
        bool canInteract = !isInBoat && distance < interactionDistance && nearestPierToPlayer != null;
        enterButton.SetActive(canInteract);
    }

    private void HandleBoatMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        boatRb.linearVelocity = new Vector2(h, v) * speed;

        if (Mathf.Abs(h) > rotationDeadzone)
        {
            bool shouldFaceLeft = h > 0f;
            if (facingLeft != shouldFaceLeft)
            {
                facingLeft = shouldFaceLeft;
                float angleY = facingLeft ? 180f : 0f;
                transform.rotation = Quaternion.Euler(0f, angleY, 0f);
                if (enterButton != null)
                {
                    enterButton.transform.rotation = enterButtonInitialRotation;
                    float shiftX = !facingLeft ? 0f : 0.18f;
                    enterButton.transform.localPosition = enterButtonInitialLocalPosition + new Vector3(shiftX, 0f, 0f);
                }

                UpdatePlayerPosition();
            }
        }
    }
    
    private void UpdatePlayerPosition()
    {
        Vector3 offset = facingLeft ? new Vector3(0.05f, 0.2f, 0) : playerOffset;
        player.position = transform.position + offset;
        player.rotation = facingLeft ? Quaternion.Euler(0f, 0f, 0f) : Quaternion.identity;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pierDetectionRadius);

        GameObject[] piers = GameObject.FindGameObjectsWithTag("Pier");
        Gizmos.color = Color.magenta;
        foreach (GameObject pier in piers)
        {
            Transform spawnPoint = pier.transform.Find("SpawnPoint");
            if (spawnPoint != null)
            {
                Gizmos.DrawWireSphere(spawnPoint.position, 0.3f);
                Gizmos.DrawLine(pier.transform.position, spawnPoint.position);
            }
        }
    }
}