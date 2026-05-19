using UnityEngine;
using System.Collections.Generic;

public class StageDatabase : MonoBehaviour {
    public static StageDatabase Instance { get; private set; }
    
    [Header("Stage Configs")]
    public List<MonsterConfig> stageConfigs = new List<MonsterConfig>();
    
    [Header("Settings")]
    public int totalStages = 100;
    public int stagesPerBoss = 10;
    public int stagesPerElite = 5;
    
    void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public MonsterConfig GetStage(int stageNumber) {
        if (stageNumber < 1 || stageNumber > stageConfigs.Count) return null;
        return stageConfigs[stageNumber - 1];
    }
    
    public int GetTotalStages() {
        return stageConfigs.Count > 0 ? stageConfigs.Count : totalStages;
    }
    
    public bool IsBossStage(int stageNumber) {
        return stageNumber % stagesPerBoss == 0;
    }
    
    public bool IsEliteStage(int stageNumber) {
        return stageNumber % stagesPerElite == 0 && !IsBossStage(stageNumber);
    }
    
    // Generate stage configs procedurally (if not assigned in Inspector)
    public void GenerateStages(int count) {
        stageConfigs.Clear();
        for (int i = 1; i <= count; i++) {
            MonsterConfig config = ScriptableObject.CreateInstance<MonsterConfig>();
            config.monsterName = i % 10 == 0 ? $"Boss {(i/10)}" : $"Enemy {i}";
            config.baseHp = 50 + (i * 15) + (i % 10 == 0 ? 200 : 0);
            config.baseAttack = 5 + (i * 3) + (i % 10 == 0 ? 20 : 0);
            config.baseDefense = 3 + (i * 1.5f) + (i % 10 == 0 ? 10 : 0);
            config.baseSpeed = 5 + (i * 0.8f) + (i % 10 == 0 ? 3 : 0);
            config.baseMagicResist = 2 + (i * 0.5f);
            config.hpPerStage = 10f;
            config.attackPerStage = 2f;
            config.defensePerStage = 1f;
            config.speedPerStage = 0.5f;
            config.isBoss = i % 10 == 0;
            config.baseGoldReward = 10 * i + (config.isBoss ? 100 : 0);
            config.baseXpReward = 5 * i + (config.isBoss ? 50 : 0);
            stageConfigs.Add(config);
        }
    }
}
