using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol")]
    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 2f;
    
    [Header("Chase")]
    public float chaseSpeed = 4f;
    public float chaseRange = 5f;
    public float returnDistance = 8f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance = 1f;
    public LayerMask groundLayerMask;
    
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    
    private Vector3 targetPoint;
    private bool movingToB = true;
    private bool isChasing = false;
    private Vector3 originalPosition;
    
    private enum EnemyState { Patrol, Chase, Return }
    private EnemyState currentState = EnemyState.Patrol;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        originalPosition = transform.position;
        
        if (pointA == null || pointB == null)
        {
            // Create default patrol points
            pointA = new GameObject("PatrolPointA").transform;
            pointB = new GameObject("PatrolPointB").transform;
            pointA.position = transform.position + Vector3.left * 3f;
            pointB.position = transform.position + Vector3.right * 3f;
        }
        
        targetPoint = pointB.position;
    }
    
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                CheckForPlayer();
                break;
            case EnemyState.Chase:
                ChasePlayer();
                CheckReturnCondition();
                break;
            case EnemyState.Return:
                ReturnToPatrol();
                break;
        }
        
        UpdateAnimation();
    }
    
    void Patrol()
    {
        // Move towards target point
        Vector3 direction = (targetPoint - transform.position).normalized;
        
        // Check if there's ground ahead
        if (IsGroundAhead())
        {
            rb.linearVelocity = new Vector2(direction.x * patrolSpeed, rb.linearVelocity.y);
        }
        else
        {
            // Turn around if no ground ahead
            SwitchTarget();
        }
        
        // Check if reached target
        if (Vector3.Distance(transform.position, targetPoint) < 0.5f)
        {
            SwitchTarget();
        }
        
        // Flip sprite based on direction
        if (direction.x > 0) spriteRenderer.flipX = false;
        else if (direction.x < 0) spriteRenderer.flipX = true;
    }
    
    void SwitchTarget()
    {
        if (movingToB)
        {
            targetPoint = pointA.position;
            movingToB = false;
        }
        else
        {
            targetPoint = pointB.position;
            movingToB = true;
        }
    }
    
    void CheckForPlayer()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRange)
        {
            currentState = EnemyState.Chase;
        }
    }
    
    void ChasePlayer()
    {
        if (player == null) return;
        
        Vector3 direction = (player.position - transform.position).normalized;
        
        // Only chase if there's ground ahead or player is at same level
        if (IsGroundAhead() || Mathf.Abs(player.position.y - transform.position.y) < 1f)
        {
            rb.linearVelocity = new Vector2(direction.x * chaseSpeed, rb.linearVelocity.y);
        }
        
        // Flip sprite based on direction
        if (direction.x > 0) spriteRenderer.flipX = false;
        else if (direction.x < 0) spriteRenderer.flipX = true;
    }
    
    void CheckReturnCondition()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > returnDistance)
        {
            currentState = EnemyState.Return;
        }
    }
    
    void ReturnToPatrol()
    {
        Vector3 direction = (originalPosition - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * patrolSpeed, rb.linearVelocity.y);
        
        // Flip sprite based on direction
        if (direction.x > 0) spriteRenderer.flipX = false;
        else if (direction.x < 0) spriteRenderer.flipX = true;
        
        // Check if returned to original position
        if (Vector3.Distance(transform.position, originalPosition) < 1f)
        {
            currentState = EnemyState.Patrol;
            // Reset target to closest patrol point
            float distanceToA = Vector3.Distance(transform.position, pointA.position);
            float distanceToB = Vector3.Distance(transform.position, pointB.position);
            
            if (distanceToA < distanceToB)
            {
                targetPoint = pointB.position;
                movingToB = true;
            }
            else
            {
                targetPoint = pointA.position;
                movingToB = false;
            }
        }
    }
    
    bool IsGroundAhead()
    {
        Vector2 rayOrigin = groundCheck.position;
        Vector2 rayDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection + Vector2.down, groundCheckDistance, groundLayerMask);
        return hit.collider != null;
    }
    
    void UpdateAnimation()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("IsChasing", currentState == EnemyState.Chase);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw patrol range
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
            Gizmos.DrawWireSphere(pointA.position, 0.3f);
            Gizmos.DrawWireSphere(pointB.position, 0.3f);
        }
        
        // Draw chase range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        // Draw return range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, returnDistance);
        
        // Draw ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Vector2 rayDirection = spriteRenderer != null && spriteRenderer.flipX ? Vector2.left : Vector2.right;
            Gizmos.DrawRay(groundCheck.position, (rayDirection + Vector2.down) * groundCheckDistance);
        }
    }
}
