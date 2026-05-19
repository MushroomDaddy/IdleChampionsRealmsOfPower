using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    
    [Header("Settings")]
    [Range(0, 1)] public float masterVolume = 1f;
    [Range(0, 1)] public float bgmVolume = 0.7f;
    [Range(0, 1)] public float sfxVolume = 1f;
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void PlayBGM(AudioClip clip) {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume * masterVolume;
        bgmSource.Play();
    }
    
    public void PlaySFX(AudioClip clip) {
        if (sfxSource == null || clip == null) return;
        sfxSource.volume = sfxVolume * masterVolume;
        sfxSource.PlayOneShot(clip);
    }
}
