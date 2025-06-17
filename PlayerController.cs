using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayerMask;
    
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    
    [Header("Audio")]
    public AudioClip jumpSound;
    public AudioClip collectSound;
    public AudioClip hurtSound;
    
    [Header("Animation Sprites")]
    public Sprite idleSprite;
    public Sprite[] walkSprites;
    public Sprite jumpSprite;
    public Sprite fallSprite;
    public float walkAnimSpeed = 0.1f;
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isGrounded;
    private bool facingRight = true;
    private Vector3 respawnPoint;
    private Vector3 lastGroundedPosition;
    public float fallThresholdY = -10f; // Ngưỡng rơi xuống vực
    
    // Animation variables
    private int currentWalkFrame = 0;
    private float walkAnimTimer = 0f;
    private bool wasGrounded = true;
    
    // Animation parameters
    private readonly int speedParam = Animator.StringToHash("Speed");
    private readonly int jumpParam = Animator.StringToHash("Jump");
    private readonly int groundedParam = Animator.StringToHash("Grounded");
    
    void Start()
    {
        // Get components with error handling
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("PlayerController: Rigidbody2D component not found!");
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("PlayerController: SpriteRenderer component not found!");
        }

        currentHealth = maxHealth;

        // Tìm vị trí mặt đất gần nhất bên dưới để đặt respawnPoint và lastGroundedPosition
        Vector3 groundPos = FindGroundBelow(transform.position);
        respawnPoint = groundPos;
        lastGroundedPosition = groundPos;
        transform.position = groundPos;

        // Initialize animation variables
        wasGrounded = true;
        currentWalkFrame = 0;
        walkAnimTimer = 0f;

        // Null check for GameManager with delayed call
        StartCoroutine(InitializeWithGameManager());

        // Create groundCheck if it doesn't exist or bị mất script
        if (groundCheck == null)
        {
            GameObject groundCheckObj = GameObject.Find("groundCheck");
            if (groundCheckObj == null)
            {
                groundCheckObj = new GameObject("groundCheck");
                groundCheckObj.transform.SetParent(transform);
                groundCheckObj.transform.localPosition = new Vector3(0, -0.5f, 0);
            }
            groundCheck = groundCheckObj.transform;
            Debug.Log("GroundCheck created or reattached automatically");
        }

        // Validate ground layer mask
        if (groundLayerMask == 0)
        {
            Debug.LogWarning("PlayerController: Ground LayerMask is not set! Setting to default layer.");
            groundLayerMask = 1; // Default layer
        }
    }

    // Hàm tìm vị trí mặt đất gần nhất bên dưới
    private Vector3 FindGroundBelow(Vector3 startPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, Vector2.down, 10f, groundLayerMask);
        if (hit.collider != null)
        {
            Vector3 groundPos = hit.point;
            groundPos.y += 0.5f; // Đặt player trên mặt đất một chút
            return groundPos;
        }
        // Nếu không tìm thấy mặt đất, giữ nguyên vị trí cũ
        return startPos;
    }
    
    private System.Collections.IEnumerator InitializeWithGameManager()
    {
        // Wait a frame to ensure GameManager is initialized
        yield return null;
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthUI(currentHealth);
        }
        else
        {
            Debug.LogWarning("GameManager not found during Player initialization");
        }
    }
    
    void Update()
    {
        HandleInput();
        CheckGrounded();
        UpdateAnimations();
        UpdateSpriteAnimation();
        CheckFallOffMap();
    }
    
    void HandleInput()
    {
        // Horizontal movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        
        // WASD alternative
        if (Input.GetKey(KeyCode.A)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D)) horizontal = 1f;
        
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        
        // Flip sprite
        if (horizontal > 0 && !facingRight) Flip();
        else if (horizontal < 0 && facingRight) Flip();
        
        // Jumping
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && isGrounded)
        {
            Jump();
        }
    }
    
    void CheckGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogError("PlayerController: GroundCheck Transform is null!");
            return;
        }

        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayerMask);

        // Nếu vừa chạm đất, lưu lại vị trí
        if (isGrounded)
        {
            lastGroundedPosition = transform.position;
        }

        // Debug ground detection
        if (wasGrounded != isGrounded)
        {
            Debug.Log($"Grounded state changed: {isGrounded} at position {groundCheck.position}");
            if (isGrounded && !wasGrounded)
            {
                OnLanding();
            }
        }
    }

    void CheckFallOffMap()
    {
        if (transform.position.y < fallThresholdY)
        {
            // Đưa nhân vật về vị trí cuối cùng trên mặt đất
            Vector3 respawnPos = lastGroundedPosition;
            respawnPos.y += 1f; // Đảm bảo đứng trên mặt đất
            transform.position = respawnPos;
            TakeDamage(1);
        }
    }
    
    void UpdateSpriteAnimation()
    {
        if (spriteRenderer == null) return;
        
        // Jump/Fall states have priority
        if (!isGrounded)
        {
            if (rb.linearVelocity.y > 0.1f)
            {
                // Jumping up
                if (jumpSprite != null)
                    spriteRenderer.sprite = jumpSprite;
            }
            else if (rb.linearVelocity.y < -0.1f)
            {
                // Falling down
                if (fallSprite != null)
                    spriteRenderer.sprite = fallSprite;
            }
        }
        else
        {
            // Ground movement animations
            float speed = Mathf.Abs(rb.linearVelocity.x);
            
            if (speed > 0.1f)
            {
                // Walking animation
                AnimateWalk();
            }
            else
            {
                // Idle animation
                if (idleSprite != null)
                    spriteRenderer.sprite = idleSprite;
            }
        }
    }
    
    void AnimateWalk()
    {
        if (walkSprites == null || walkSprites.Length == 0) return;
        
        walkAnimTimer += Time.deltaTime;
        
        if (walkAnimTimer >= walkAnimSpeed)
        {
            walkAnimTimer = 0f;
            currentWalkFrame = (currentWalkFrame + 1) % walkSprites.Length;
            
            if (walkSprites[currentWalkFrame] != null)
                spriteRenderer.sprite = walkSprites[currentWalkFrame];
        }
    }
    
    void Jump()
    {
        if (!isGrounded) return;
        
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        
        // Immediately set jump sprite
        if (jumpSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = jumpSprite;
        }
        
        if (jumpSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(jumpSound);
        }
        
        Debug.Log("Jump executed!");
        OnJumpStart();
    }
    
    void OnJumpStart()
    {
        // Additional jump start effects can be added here
        Debug.Log("Jump animation started");
    }
    
    void OnLanding()
    {
        // Landing effects can be added here
        Debug.Log("Player landed");
        
        // Reset walk animation frame
        currentWalkFrame = 0;
        walkAnimTimer = 0f;
    }
    
    void UpdateAnimations()
    {
        if (animator != null)
        {
            animator.SetFloat(speedParam, Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool(groundedParam, isGrounded);
            animator.SetBool(jumpParam, !isGrounded);
        }
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthUI(currentHealth);
        }
        
        if (hurtSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(hurtSound);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateHealthUI(currentHealth);
        }
    }
    
    void Die()
    {
        // Reset coin về 0 và restart màn chơi
        if (GameManager.Instance != null)
        {
            GameManager.Instance.score = 0;
            GameManager.Instance.UpdateScoreUI();
            GameManager.Instance.RestartGame();
        }
        else
        {
            // Nếu không có GameManager, chỉ reset máu và vị trí
            transform.position = respawnPoint;
            currentHealth = maxHealth;
        }
    }
    
    public void SetRespawnPoint(Vector3 newRespawnPoint)
    {
        respawnPoint = newRespawnPoint;
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}