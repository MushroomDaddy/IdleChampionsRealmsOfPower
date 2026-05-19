using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;
    public AudioSource battleSource; // For layered battle SFX
    
    [Header("BGM Clips")]
    public AudioClip bgmTown;
    public AudioClip bgmBattle;
    public AudioClip bgmBattleBoss;
    public AudioClip bgmBattleRaid;
    public AudioClip bgmDungeon;
    
    [Header("Battle SFX")]
    public AudioClip sfxHit;
    public AudioClip sfxCrit;
    public AudioClip sfxMiss;
    public AudioClip sfxDodge;
    public AudioClip sfxBlock;
    public AudioClip sfxEnemyHit;
    public AudioClip sfxDeath;
    public AudioClip sfxBossEnrage;
    public AudioClip sfxVictory;
    public AudioClip sfxDefeat;
    
    [Header("UI SFX")]
    public AudioClip sfxButtonClick;
    public AudioClip sfxButtonHover;
    public AudioClip sfxEquip;
    public AudioClip sfxUnequip;
    public AudioClip sfxLevelUp;
    public AudioClip sfxGold;
    public AudioClip sfxGem;
    public AudioClip sfxNewItem;
    
    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float bgmVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    public bool isMuted = false;
    
    // Enhanced: Audio categories for dynamic mixing
    private Dictionary<string, AudioClip> bgmClips = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();
    private Queue<AudioClip> battleSFXQueue = new Queue<AudioClip>(); // Enhanced: Layered SFX
    
    void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeAudio();
    }
    
    void InitializeAudio() {
        // Create audio sources if not assigned
        if (bgmSource == null) {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume * masterVolume;
        }
        if (sfxSource == null) {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.volume = sfxVolume * masterVolume;
        }
        if (uiSource == null) {
            uiSource = gameObject.AddComponent<AudioSource>();
            uiSource.loop = false;
            uiSource.volume = sfxVolume * masterVolume;
        }
        if (battleSource == null) {
            battleSource = gameObject.AddComponent<AudioSource>();
            battleSource.loop = false;
            battleSource.volume = sfxVolume * masterVolume;
        }
    }
    
    // Enhanced: Load audio clips from extracted APK assets
    public void LoadAudioFromAPK(string audioPath) {
        // This would load the MP3 files from /root/apk_full_analysis/audio/
        // Unity can't directly load MP3 at runtime without plugins, but we can convert to OGG
        // For Editor, we'll create references manually
        Debug.Log($"Loading audio from {audioPath}...");
    }
    
    // BGM Control
    public void PlayBGM(string bgmName) {
        if (isMuted) return;
        
        AudioClip clip = null;
        switch (bgmName.ToLower()) {
            case "town": clip = bgmTown; break;
            case "battle": clip = bgmBattle; break;
            case "boss": clip = bgmBattleBoss; break;
            case "raid": clip = bgmBattleRaid; break;
            case "dungeon": clip = bgmDungeon; break;
        }
        
        if (clip != null && bgmSource.clip != clip) {
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }
    
    public void StopBGM() => bgmSource.Stop();
    
    // SFX Control
    public void PlaySFX(string sfxName) {
        if (isMuted) return;
        
        AudioClip clip = null;
        switch (sfxName.ToLower()) {
            case "hit": clip = sfxHit; break;
            case "crit": clip = sfxCrit; break;
            case "miss": clip = sfxMiss; break;
            case "dodge": clip = sfxDodge; break;
            case "block": clip = sfxBlock; break;
            case "enemyhit": clip = sfxEnemyHit; break;
            case "death": clip = sfxDeath; break;
            case "enrage": clip = sfxBossEnrage; break;
            case "victory": clip = sfxVictory; break;
            case "defeat": clip = sfxDefeat; break;
            case "click": clip = sfxButtonClick; break;
            case "hover": clip = sfxButtonHover; break;
            case "equip": clip = sfxEquip; break;
            case "levelup": clip = sfxLevelUp; break;
            case "gold": clip = sfxGold; break;
            case "gem": clip = sfxGem; break;
            case "newitem": clip = sfxNewItem; break;
        }
        
        if (clip != null) {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    // Enhanced: Layered battle SFX (hit combos, etc.)
    public void QueueBattleSFX(AudioClip clip) {
        if (clip != null && !isMuted) {
            battleSFXQueue.Enqueue(clip);
            if (!battleSource.isPlaying && battleSFXQueue.Count > 0) {
                StartCoroutine(PlayBattleSFXQueue());
            }
        }
    }
    
    private IEnumerator PlayBattleSFXQueue() {
        while (battleSFXQueue.Count > 0) {
            AudioClip clip = battleSFXQueue.Dequeue();
            battleSource.PlayOneShot(clip);
            yield return new WaitForSeconds(clip.length);
        }
    }
    
    // Volume Controls
    public void SetMasterVolume(float volume) {
        masterVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume * masterVolume;
        sfxSource.volume = sfxVolume * masterVolume;
        uiSource.volume = sfxVolume * masterVolume;
        battleSource.volume = sfxVolume * masterVolume;
    }
    
    public void SetBGMVolume(float volume) {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume * masterVolume;
    }
    
    public void SetSFXVolume(float volume) {
        sfxVolume = Mathf.Clamp01(volume);
        sfxSource.volume = sfxVolume * masterVolume;
        uiSource.volume = sfxVolume * masterVolume;
        battleSource.volume = sfxVolume * masterVolume;
    }
    
    public void ToggleMute() {
        isMuted = !isMuted;
        bgmSource.mute = isMuted;
        sfxSource.mute = isMuted;
        uiSource.mute = isMuted;
        battleSource.mute = isMuted;
    }
}
