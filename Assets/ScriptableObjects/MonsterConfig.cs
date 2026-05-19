using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewMonster", menuName = "IdleChampions/Monster Config")]
public class MonsterConfig : ScriptableObject {
    [Header("Identity")]
    public string monsterName;
    public MonsterType type = MonsterType.Normal;
    public Sprite portrait;
    public GameObject prefab; // 3D model or 2D sprite
    
    [Header("Base Stats")]
    public float baseHp = 50f;
    public float baseAttack = 5f;
    public float baseDefense = 3f;
    public float baseSpeed = 5f;
    public float baseMagicResist = 2f;
    
    [Header("Scaling Per Stage")]
    public float hpPerStage = 10f;
    public float attackPerStage = 2f;
    public float defensePerStage = 1f;
    public float speedPerStage = 0.5f;
    
    [Header("Special Mechanics")]
    public bool isBoss = false;
    public bool hasEnrage = false; // Boss enrages at low HP
    public float enrageHpThreshold = 0.3f; // Enrage at 30% HP
    public float enrageDamageMultiplier = 1.5f;
    public bool hasShield = false; // Temporary shield
    public float shieldAmount = 0f;
    public bool isUndead = false; // Takes bonus damage from holy
    public bool isDemon = false; // Takes bonus from angel
    public string specialAbility; // Name of special skill
    
    [Header("Loot")]
    public List<DropTableEntry> dropTable = new List<DropTableEntry>();
    public int baseGoldReward = 10;
    public int baseXpReward = 5;
    public float legendaryDropChance = 0.001f; // 0.1%
    public float epicDropChance = 0.01f; // 1%
    public float rareDropChance = 0.05f; // 5%
    
    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip deathSound;
    public AudioClip hurtSound;
    
    // Calculate stats for a given stage
    public (float hp, float atk, float def, float spd) GetStatsForStage(int stage) {
        return (
            baseHp + (hpPerStage * stage),
            baseAttack + (attackPerStage * stage),
            baseDefense + (defensePerStage * stage),
            baseSpeed + (speedPerStage * stage)
        );
    }
}

public enum MonsterType { Normal, Elite, Boss, MiniBoss, RaidBoss }

[System.Serializable]
public class DropTableEntry {
    public string itemId;
    public float dropChance;
    public int minQuantity = 1;
    public int maxQuantity = 1;
}
