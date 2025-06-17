using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public int keysRequired = 1;
    public GameObject doorSprite;
    public AudioClip openSound;
    
    [Header("UI")]
    public GameObject interactionPrompt;
    
    private bool isPlayerNear = false;
    private bool isOpen = false;
    private Collider2D doorCollider;
    
    void Start()
    {
        doorCollider = GetComponent<Collider2D>();
        
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void Update()
    {
        if (isPlayerNear && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            TryOpenDoor();
        }
    }
    
    void TryOpenDoor()
    {
        if (GameManager.Instance.keys >= keysRequired)
        {
            // Use keys
            for (int i = 0; i < keysRequired; i++)
            {
                GameManager.Instance.UseKey();
            }
            
            OpenDoor();
        }
        else
        {
            // Show message that more keys are needed
            Debug.Log($"Need {keysRequired} keys to open this door. You have {GameManager.Instance.keys}");
        }
    }
    
    void OpenDoor()
    {
        isOpen = true;
        
        // Disable collider
        if (doorCollider != null)
            doorCollider.enabled = false;
        
        // Hide door sprite
        if (doorSprite != null)
            doorSprite.SetActive(false);
        
        // Play sound
        if (openSound != null)
            AudioManager.Instance.PlaySFX(openSound);
        
        // Hide interaction prompt
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            isPlayerNear = true;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(true);
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (interactionPrompt != null)
                interactionPrompt.SetActive(false);
        }
    }
}
