using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPet", menuName = "IdleChampions/Pet Config")]
public class PetConfig : ScriptableObject {
    [Header("Identity")]
    public string petId;
    public string petName;
    public PetRarity rarity;
    public PetType type;
    public Sprite icon;
    public GameObject prefab; // 3D model or 2D sprite
    
    [Header("Base Stats")]
    public float baseAttack;
    public float baseDefense;
    public float baseSpeed;
    public float baseHp;
    public float baseSpecial; // Depends on type
    
    [Header("Scaling")]
    public float attackPerLevel = 2f;
    public float defensePerLevel = 1f;
    public float speedPerLevel = 0.5f;
    public float hpPerLevel = 5f;
    
    [Header("Evolution (Enhanced)")]
    public int maxEvolutionLevel = 5; // 1-5 stars
    public int currentEvolution = 1;
    public List<EvolutionRequirement> evolutionRequirements = new List<EvolutionRequirement>();
    
    [Header("Skill Tree (Enhanced)")]
    public List<PetSkillNode> skillTree = new List<PetSkillNode>();
    public int availableSkillPoints = 0;
    
    [Header("Special Ability")]
    public string specialSkillId;
    public float specialSkillCooldown = 30f; // Seconds
    public bool isPassive = false;
    
    [Header("Fusion (Enhanced)")]
    public List<string> fusionRecipes = new List<string>(); // petId + petId = result
    public bool canFuse = false;
    public string fusedPetId; // Result of fusion
    
    [Header("Collection")]
    public int shardCount = 0;
    public int shardsToSummon = 50; // Shards needed to get this pet
    public bool isUnlocked = false;
    public bool isFavorite = false;
    
    [Header("Combat")]
    public float joinBattleChance = 0.5f; // 50% chance to join battle
    public float battleDuration = 10f; // How long pet fights
    public AIType petAI = AIType.Balanced;
    
    // Get stats for a given level
    public (float atk, float def, float spd, float hp) GetStats(int level) {
        return (
            baseAttack + (attackPerLevel * level),
            baseDefense + (defensePerLevel * level),
            baseSpeed + (speedPerLevel * level),
            baseHp + (hpPerLevel * level)
        );
    }
    
    // Evolution check
    public bool CanEvolve(int currentShards) {
        if (currentEvolution >= maxEvolutionLevel) return false;
        var req = evolutionRequirements.Find(r => r.evolutionLevel == currentEvolution + 1);
        return req != null && currentShards >= req.shardsRequired;
    }
    
    // Evolve (Enhanced: Visual transformation)
    public bool Evolve() {
        if (currentEvolution >= maxEvolutionLevel) return false;
        currentEvolution++;
        // Unlock new skill node
        if (currentEvolution <= skillTree.Count) {
            skillTree[currentEvolution - 1].unlocked = true;
        }
        return true;
    }
}

[System.Serializable]
public class EvolutionRequirement {
    public int evolutionLevel; // 2, 3, 4, 5
    public int shardsRequired;
    public int goldRequired;
    public List<string> itemRequirements = new List<string>();
}

[System.Serializable]
public class PetSkillNode {
    public string skillId;
    public string skillName;
    public string description;
    public bool unlocked = false;
    public int skillPointCost = 1;
    public PetSkillNode prerequisite; // Skill tree linkage
    public float effectValue; // Damage multiplier, etc.
    public SkillEffectType effectType;
}

public enum AIType { Aggressive, Defensive, Balanced, Support }

public enum SkillEffectType { DamageBoost, DefenseBoost, SpeedBoost, Healing, Thorns, Lifesteal, CritBoost, DodgeBoost }
