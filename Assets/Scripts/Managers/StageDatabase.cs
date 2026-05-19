using UnityEngine;

public class StageDatabase : MonoBehaviour {
    public static StageDatabase Instance { get; private set; }
    
    [Header("Stage Configs")]
    public StageConfig[] allStages;
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public StageConfig GetStage(int stageNumber) {
        if (allStages == null || allStages.Length == 0) return null;
        int index = stageNumber - 1;
        if (index >= 0 && index < allStages.Length) return allStages[index];
        return null;
    }
    
    public int TotalStages => allStages != null ? allStages.Length : 0;
}
