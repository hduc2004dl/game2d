using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public CollectibleType type;
    public int value = 10;
    public GameObject collectEffect;
    
    [Header("Animation")]
    public float rotationSpeed = 90f;
    public float bobHeight = 0.5f;
    public float bobSpeed = 2f;
    
    private Vector3 startPosition;
    private bool isCollected = false;
    
    public enum CollectibleType
    {
        Coin,
        Key
    }
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    void Update()
    {
        if (!isCollected)
        {
            // Rotate the collectible
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            
            // Bob up and down
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            Collect();
        }
    }

    public void Collect()
    {
        if (isCollected) return;
        
        isCollected = true;
        
        try
        {
            // Add to game manager
            if (GameManager.Instance != null)
            {
                switch (type)
                {
                    case CollectibleType.Coin:
                        GameManager.Instance.AddScore(value);
                        // Play coin sound
                        if (AudioManager.Instance != null && AudioManager.Instance.collectCoinSound != null)
                        {
                            AudioManager.Instance.PlaySFX(AudioManager.Instance.collectCoinSound);
                        }
                        break;
                    case CollectibleType.Key:
                        GameManager.Instance.AddKey();
                        // Play key sound
                        if (AudioManager.Instance != null && AudioManager.Instance.collectKeySound != null)
                        {
                            AudioManager.Instance.PlaySFX(AudioManager.Instance.collectKeySound);
                        }
                        break;
                }
            }
            else
            {
                Debug.LogWarning("GameManager not found when collecting item");
            }
            
            // Spawn effect
            if (collectEffect != null)
            {
                Instantiate(collectEffect, transform.position, Quaternion.identity);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error during collection: {e.Message}");
        }
        finally
        {
            // Always destroy the collectible
            Destroy(gameObject);
        }
    }
}
