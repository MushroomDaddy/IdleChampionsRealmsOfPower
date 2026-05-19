using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnhancedHeroData {
    // Core Identity
    public string heroName;
    public HeroClass classType;
    public int level = 1;
    public float xp = 0;
    public float xpToNextLevel = 100;
    
    // Primary Stats (Enhanced: More granular)
    public float hp;
    public float maxHp;
    public float attack;
    public float magic;
    public float defense;
    public float magicDefense;
    public float speed;
    
    // Secondary Stats (Enhanced: More depth)
    public float critRate = 0.05f;      // 5% base
    public float critDamage = 1.5f;      // 150% damage
    public float dodge = 0.03f;          // 3% chance
    public float accuracy = 0.95f;        // 95% hit chance
    public float blockChance = 0.05f;    // New: Block chance
    public float blockReduction = 0.3f;  // New: Damage reduction when blocking
    public float lifesteal = 0f;          // New: HP drain on hit
    public float thorns = 0f;             // New: Reflect damage
    public float cooldownReduction = 0f;  // New: Skill haste
    
    // Advanced Stats (Enhanced: Endgame scaling)
    public float powerScore;
    public float combatRating;  // New: Overall power rating
    public int skillPoints = 0; // New: For skill tree
    public int talentPoints = 0; // New: For talent system
    
    // Equipment & Inventory
    public List<string> equippedGearIds = new List<string>();
    public List<string> inventoryGearIds = new List<string>();
    public List<string> equippedSkillIds = new List<string>(); // New: Skill loadout
    
    // Progression (Enhanced: More systems)
    public int talentTreeUnlocks = 0;
    public float totalDamageDealt = 0;
    public float totalDamageTaken = 0;
    public int totalBattlesWon = 0;
    public int totalBattlesLost = 0;
    public float longestSurvivalTime = 0;
    
    // Class-specific bonuses (Enhanced: Unique perks)
    public void LevelUp() {
        level++;
        xp -= xpToNextLevel;
        xpToNextLevel *= 1.25f; // Slightly slower scaling than APK
        
        // Base stat growth
        switch (classType) {
            case HeroClass.Knight:
                maxHp += 25;        // Tankier
                defense += 4;
                magicDefense += 2;
                attack += 2;
                blockChance += 0.005f;
                break;
            case HeroClass.Ranger:
                attack += 4;
                speed += 3;
                dodge += 0.015f;
                accuracy += 0.01f;
                critRate += 0.005f;
                break;
            case HeroClass.Mage:
                magic += 5;
                magicDefense += 3;
                critDamage += 0.15f;
                lifesteal += 0.01f;
                break;
        }
        
        // Random bonus (Enhanced: RNG element)
        System.Random rng = new System.Random();
        int bonus = rng.Next(0, 3);
        switch (bonus) {
            case 0: maxHp += 5; break;
            case 1: attack += 1; magic += 1; break;
            case 2: speed += 1; break;
        }
        
        CalculatePowerScore();
        hp = maxHp; // Full heal on level up
    }
    
    public void CalculatePowerScore() {
        // Enhanced formula (more weighting on advanced stats)
        powerScore = (maxHp * 0.1f) 
            + attack + magic 
            + (defense * 1.2f) + (magicDefense * 1.2f) 
            + (speed * 0.8f) 
            + (critRate * 150) + (critDamage * 15) 
            + (dodge * 20) + (accuracy * 10)
            + (blockChance * 30) + (lifesteal * 50);
        
        combatRating = powerScore * (1 + (level * 0.1f));
    }
    
    // Enhanced: Take damage with all mechanics
    public float TakeDamage(float incomingDamage, bool isMagicDamage = false, bool isTrueDamage = false) {
        float finalDamage = incomingDamage;
        
        // Block check
        if (!isMagicDamage && !isTrueDamage && UnityEngine.Random.value < blockChance) {
            finalDamage *= (1 - blockReduction);
            return -finalDamage; // Negative indicates blocked
        }
        
        // Dodge check (only for physical)
        if (!isMagicDamage && !isTrueDamage && UnityEngine.Random.value < dodge) {
            return 0; // Dodged
        }
        
        // Defense reduction
        if (!isTrueDamage) {
            if (isMagicDamage) {
                finalDamage = Mathf.Max(1, finalDamage - (magicDefense * 0.6f));
            } else {
                finalDamage = Mathf.Max(1, finalDamage - (defense * 0.5f));
            }
        }
        
        hp = Mathf.Max(0, hp - finalDamage);
        totalDamageTaken += finalDamage;
        return finalDamage;
    }
    
    // Enhanced: Deal damage with crits
    public (float damage, bool isCrit, bool isMiss) DealDamage(bool isMagic = false) {
        if (UnityEngine.Random.value > accuracy) {
            return (0, false, true); // Miss
        }
        
        float baseDmg = isMagic ? magic : attack;
        float dmg = baseDmg;
        
        bool isCrit = UnityEngine.Random.value < critRate;
        if (isCrit) {
            dmg *= critDamage;
        }
        
        totalDamageDealt += dmg;
        return (dmg, isCrit, false);
    }
}
