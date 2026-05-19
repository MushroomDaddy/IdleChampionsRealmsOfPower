using System;
using System.Collections.Generic;

[Serializable]
public class SaveData {
    public string heroName;
    public EnhancedHeroData hero;
    public int gold = 0;
    public int gems = 0;
    public int highestStage = 1;
    public long lastLogoutTimestamp;
    public int saveVersion = 2;
    public float totalPlayTime = 0;
    public int achievementPoints = 0;
    
    // Inventory
    public List<string> inventoryGearIds = new List<string>();
    public List<string> petIds = new List<string>();
    public List<string> unlockedSkillIds = new List<string>();
    
    // Settings
    public float masterVolume = 1f;
    public float bgmVolume = 0.7f;
    public float sfxVolume = 1f;
    public bool isMuted = false;
    public float battleSpeed = 1f;
    
    // Statistics
    public int totalBattlesWon = 0;
    public int totalBattlesLost = 0;
    public float totalGoldEarned = 0;
    public float totalXpEarned = 0;
}
