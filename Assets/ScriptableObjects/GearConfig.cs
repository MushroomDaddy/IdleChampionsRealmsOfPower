using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGear", menuName = "IdleChampions/Gear Config")]
public class GearConfig : ScriptableObject {
    [Header("Identity")]
    public string gearId;
    public string gearName;
    public GearSlot slot;
    public GearRarity rarity;
    public Sprite icon;
    public GameObject prefab; // 3D model or 2D sprite
    
    [Header("Enhancement")]
    [Range(0, 15)] public int enhanceLevel = 0; // Enhanced: Up to +15 (was +10)
    public int maxEnhanceLevel = 15;
    public int enhancementCostGold = 100;
    public float enhancementSuccessRate = 1.0f; // 100% for +0-+5, decreases later
    
    [Header("Base Stats")]
    public Dictionary<StatType, float> baseStatBonuses = new Dictionary<StatType, float>();
    
    [Header("Set Bonus (Enhanced)")]
    public string setName; // e.g., "Warrior's Set"
    public int totalSetPieces = 6; // How many pieces in full set
    public List<SetBonus> setBonuses = new List<SetBonus>(); // Bonuses for 2/4/6 pieces
    
    [Header("Socket System (Enhanced)")]
    public int maxSockets = 0; // Unlocked at +10 enhancement
    public List<SocketData> sockets = new List<SocketData>();
    public bool hasSocket = false;
    
    [Header("Random Stats (Enhanced)")]
    public List<RandomStat> randomStats = new List<RandomStat>(); // Additional rng stats
    
    [Header("Economy")]
    public int sellValue;
    public int upgradeCostMultiplier = 1;
    
    [Header("Visuals")]
    public Color rarityColor = Color.white;
    public GameObject pickupEffect;
    public AudioClip equipSound;
    
    // Enhanced: Calculate total stats including enhancement, set bonuses, sockets
    public Dictionary<StatType, float> GetTotalStats() {
        var total = new Dictionary<StatType, float>();
        
        // Base stats
        foreach (var stat in baseStatBonuses) {
            float value = stat.Value * (1 + (enhanceLevel * 0.1f)); // +10% per enhance level
            total[stat.Key] = value;
        }
        
        // Set bonuses (applied elsewhere when checking equipped set)
        // Sockets
        foreach (var socket in sockets) {
            if (socket.isFilled && socket.gemStatBonus != null) {
                foreach (var stat in socket.gemStatBonus) {
                    if (total.ContainsKey(stat.Key)) total[stat.Key] += stat.Value;
                    else total[stat.Key] = stat.Value;
                }
            }
        }
        
        // Random stats
        foreach (var randomStat in randomStats) {
            if (randomStat.isUnlocked) {
                if (total.ContainsKey(randomStat.statType)) total[randomStat.statType] += randomStat.value;
                else total[randomStat.statType] = randomStat.value;
            }
        }
        
        return total;
    }
    
    // Enhanced: Enhancement success calculation
    public bool TryEnhance() {
        if (enhanceLevel >= maxEnhanceLevel) return false;
        
        float roll = UnityEngine.Random.value;
        if (roll <= enhancementSuccessRate) {
            enhanceLevel++;
            // Reduce success rate for next level
            if (enhanceLevel > 10) enhancementSuccessRate *= 0.8f;
            else if (enhanceLevel > 5) enhancementSuccessRate *= 0.9f;
            return true;
        }
        return false;
    }
    
    // Unlock socket (at +10)
    public void UnlockSocket() {
        if (enhanceLevel >= 10 && !hasSocket && sockets.Count < maxSockets) {
            hasSocket = true;
            sockets.Add(new SocketData { socketIndex = sockets.Count, isFilled = false });
        }
    }
}

[System.Serializable]
public class SetBonus {
    public int piecesRequired; // 2, 4, 6
    public Dictionary<StatType, float> bonusStats = new Dictionary<StatType, float>();
    public string description;
}

[System.Serializable]
public class SocketData {
    public int socketIndex;
    public bool isFilled;
    public GemType gemType;
    public Dictionary<StatType, float> gemStatBonus;
}

[System.Serializable]
public class RandomStat {
    public StatType statType;
    public float value;
    public bool isUnlocked;
    public int unlockLevel; // At which enhancement level this unlocks
}

public enum GemType { Red, Blue, Green, Yellow, Purple } // ATK, MAG, DEF, SPD, ALL
