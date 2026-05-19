using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewSkill", menuName = "IdleChampions/Skill Config")]
public class SkillConfig : ScriptableObject {
    [Header("Identity")]
    public string skillId;
    public string skillName;
    public SkillType type;
    public Sprite icon;
    public GameObject effectPrefab; // Particle effect
    
    [Header("Damage/Effect")]
    public float damageMultiplier = 1.0f; // 1.0 = 100% attack
    public float cooldown = 3f; // Seconds
    public bool isPassive = false;
    public bool isAOE = false; // Hits all enemies
    public int maxTargets = 1;
    
    [Header("Stat Modifiers (Enhanced)")]
    public List<StatModifier> statModifiers = new List<StatModifier>();
    public float duration = 0f; // 0 = instant, >0 = buff duration
    
    [Header("Element (Enhanced)")]
    public ElementType element = ElementType.None;
    public float elementMultiplier = 1.5f; // Bonus vs weak elements
    
    [Header("Skill Tree (Enhanced)")]
    public List<SkillUpgrade> upgrades = new List<SkillUpgrade>();
    public int currentUpgradeLevel = 0;
    public int maxUpgradeLevel = 5;
    
    [Header("Audio")]
    public AudioClip castSound;
    public AudioClip hitSound;
    
    // Calculate damage for given attacker
    public float CalculateDamage(float attackPower, bool isMagic) {
        float baseDmg = attackPower * damageMultiplier;
        if (isMagic) baseDmg *= 1.2f; // Magic bonus
        return baseDmg;
    }
    
    // Check if skill is ready (cooldown)
    public bool IsReady(float lastCastTime) {
        return Time.time >= lastCastTime + cooldown;
    }
    
    // Upgrade skill
    public bool Upgrade() {
        if (currentUpgradeLevel >= maxUpgradeLevel) return false;
        currentUpgradeLevel++;
        if (currentUpgradeLevel <= upgrades.Count) {
            ApplyUpgrade(upgrades[currentUpgradeLevel - 1]);
        }
        return true;
    }
    
    void ApplyUpgrade(SkillUpgrade upgrade) {
        damageMultiplier += upgrade.damageBonus;
        cooldown -= upgrade.cooldownReduction;
        duration += upgrade.durationBonus;
    }
}

[System.Serializable]
public class StatModifier {
    public StatType stat;
    public float value;
    public ModifierType modifierType = ModifierType.Additive;
}

[System.Serializable]
public class SkillUpgrade {
    public string upgradeName;
    public float damageBonus = 0f;
    public float cooldownReduction = 0f;
    public float durationBonus = 0f;
    public List<StatModifier> additionalModifiers = new List<StatModifier>();
}

public enum SkillType { Active, Passive, Toggle, Ultimate }
public enum ElementType { None, Fire, Water, Earth, Air, Light, Dark }
public enum ModifierType { Additive, Multiplicative }
