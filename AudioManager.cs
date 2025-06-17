using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Music")]
    public AudioClip backgroundMusic;
    
    [Header("Sound Effects")]
    public AudioClip jumpSound;
    public AudioClip collectCoinSound;
    public AudioClip collectKeySound;
    public AudioClip enemyHitSound;
    public AudioClip playerHurtSound;
    public AudioClip doorOpenSound;
    
    [Header("Settings")]
    [Range(0f, 1f)] public float musicVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    
    void Awake()
    {
        // Singleton pattern with validation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("AudioManager initialized");
        }
        else if (Instance != this)
        {
            Debug.Log("Duplicate AudioManager detected, destroying...");
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        try
        {
            InitializeAudioSources();
            Debug.Log("AudioManager setup complete");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AudioManager initialization failed: {e.Message}");
        }
    }

    private void InitializeAudioSources()
    {
        // Create audio sources if they don't exist
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("Music Source");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFX Source");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
        }
        
        // Set volumes
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        
        // Play background music
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }
    
    public void PlayMusic(AudioClip clip)
    {
        try
        {
            if (musicSource != null && clip != null)
            {
                musicSource.clip = clip;
                musicSource.Play();
            }
            else
            {
                if (musicSource == null) Debug.LogWarning("Music AudioSource is null");
                if (clip == null) Debug.LogWarning("Music clip is null");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error playing music: {e.Message}");
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        try
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
            else
            {
                if (sfxSource == null) Debug.LogWarning("SFX AudioSource is null");
                if (clip == null) Debug.LogWarning("Audio clip is null");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error playing SFX: {e.Message}");
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }
    
    public void ToggleMusic()
    {
        if (musicSource != null)
        {
            if (musicSource.isPlaying)
                musicSource.Pause();
            else
                musicSource.UnPause();
        }
    }
}
