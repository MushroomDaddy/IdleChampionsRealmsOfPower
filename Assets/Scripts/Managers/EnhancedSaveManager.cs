using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class EnhancedSaveManager : MonoBehaviour {
    public static EnhancedSaveManager Instance { get; private set; }
    public SaveData CurrentSave { get; private set; }
    
    private string savePath => Path.Combine(Application.persistentDataPath, "save.json");
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadSave();
    }
    
    public void LoadSave() {
        if (File.Exists(savePath)) {
            try {
                string json = File.ReadAllText(savePath);
                CurrentSave = JsonUtility.FromJson<SaveData>(json);
                if (CurrentSave == null) CreateNewSave();
            } catch {
                CreateNewSave();
            }
        } else {
            CreateNewSave();
        }
    }
    
    void CreateNewSave() {
        CurrentSave = new SaveData();
        CurrentSave.heroName = "";
        CurrentSave.gold = 0;
        CurrentSave.currentStage = 1;
    }
    
    public void SaveSave() {
        if (CurrentSave == null) return;
        string json = JsonUtility.ToJson(CurrentSave, true);
        File.WriteAllText(savePath, json);
    }
    
    public void ResetSave() {
        if (File.Exists(savePath)) File.Delete(savePath);
        CreateNewSave();
    }
}
