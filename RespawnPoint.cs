using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [Header("Checkpoint Settings")]
    public bool isActivated = false;
    public GameObject activatedEffect;
    public AudioClip activationSound;
    
    [Header("Visual")]
    public SpriteRenderer flagSprite;
    public Color inactiveColor = Color.gray;
    public Color activeColor = Color.green;
    
    void Start()
    {
        UpdateVisual();
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            ActivateCheckpoint(other.GetComponent<PlayerController>());
        }
    }
    
    void ActivateCheckpoint(PlayerController player)
    {
        isActivated = true;
        
        // Set as respawn point
        if (player != null)
        {
            player.SetRespawnPoint(transform.position);
        }
        
        // Visual feedback
        UpdateVisual();
        
        // Spawn effect
        if (activatedEffect != null)
        {
            Instantiate(activatedEffect, transform.position, Quaternion.identity);
        }
        
        // Play sound
        if (activationSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(activationSound);
        }
        
        Debug.Log("Checkpoint activated!");
    }
    
    void UpdateVisual()
    {
        if (flagSprite != null)
        {
            flagSprite.color = isActivated ? activeColor : inactiveColor;
        }
    }
}
