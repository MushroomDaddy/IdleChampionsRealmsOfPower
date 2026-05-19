using System.IO;
using UnityEngine;
using System;

[DefaultExecutionOrder(-100)]
public class EnhancedSaveManager : MonoBehaviour {
    public static EnhancedSaveManager Instance { get; private set; }
    public SaveData CurrentSave { get; private set; }
    public bool IsCloudSaveEnabled { get; private set; } = false; // Enhanced: Cloud save placeholder
    
    private string savePath => Path.Combine(Application.persistentDataPath, "save_enhanced.json");
    private string backupPath => Path.Combine(Application.persistentDataPath, "save_backup.json");
    
    [Header("Settings")]
    public bool enableAutoSave = true;
    public float autoSaveInterval = 30f; // Every 30 seconds
    private float autoSaveTimer = 0f;
    
    // Events
    public event Action OnSaveCompleted;
    public event Action OnLoadCompleted;
    public event Action<string> OnSaveError;
    
    void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSave();
    }
    
    void Update() {
        if (enableAutoSave) {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval) {
                autoSaveTimer = 0f;
                SaveSave();
            }
        }
    }
    
    public void SaveSave() {
        try {
            CurrentSave.lastLogoutTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            CurrentSave.saveVersion = 2; // Enhanced: Version 2 save format
            CurrentSave.totalPlayTime += autoSaveTimer;
            
            string json = JsonUtility.ToJson(CurrentSave, true);
            File.WriteAllText(savePath, json);
            
            // Backup
            File.Copy(savePath, backupPath, true);
            
            OnSaveCompleted?.Invoke();
            Debug.Log("Game saved successfully");
        } catch (Exception e) {
            OnSaveError?.Invoke(e.Message);
            Debug.LogError($"Save failed: {e.Message}");
        }
    }
    
    public void LoadSave() {
        try {
            if (File.Exists(savePath)) {
                string json = File.ReadAllText(savePath);
                CurrentSave = JsonUtility.FromJson<SaveData>(json);
                if (CurrentSave == null) CurrentSave = new SaveData();
                if (CurrentSave.hero == null) CurrentSave.hero = new EnhancedHeroData();
                // Enhanced: Version check
                if (CurrentSave.saveVersion < 2) MigrateSaveData();
            } else {
                CurrentSave = new SaveData();
            }
            OnLoadCompleted?.Invoke();
        } catch {
            // Try backup
            if (File.Exists(backupPath)) {
                try {
                    string json = File.ReadAllText(backupPath);
                    CurrentSave = JsonUtility.FromJson<SaveData>(json);
                    Debug.LogWarning("Loaded backup save");
                } catch {
                    CurrentSave = new SaveData();
                }
            } else {
                CurrentSave = new SaveData();
            }
        }
    }
    
    void MigrateSaveData() {
        // Enhanced: Migrate from version 1 to 2
        Debug.Log("Migrating save data to version 2...");
        // Add new fields with defaults
        CurrentSave.totalPlayTime = 0;
        CurrentSave.achievementPoints = 0;
        SaveSave();
    }
    
    public void ResetSave() {
        if (File.Exists(savePath)) File.Delete(savePath);
        if (File.Exists(backupPath)) File.Delete(backupPath);
        CurrentSave = new SaveData();
        SaveSave();
        Debug.Log("Save reset complete");
    }
    
    public void EnableCloudSave() {
        // Enhanced: Placeholder for cloud save integration
        IsCloudSaveEnabled = true;
        Debug.Log("Cloud save enabled (placeholder)");
    }
    
    public void SyncCloudSave() {
        if (!IsCloudSaveEnabled) return;
        // Enhanced: Cloud sync logic would go here
        Debug.Log("Cloud sync (placeholder)");
    }
}
