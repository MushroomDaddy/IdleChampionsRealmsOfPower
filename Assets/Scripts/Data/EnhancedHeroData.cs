using System;
using System.Collections.Generic;

[Serializable]
public class EnhancedHeroData {
    public string heroName;
    public HeroClass classType;
    public int level = 1;
    public float xp = 0;
    public float xpToNextLevel = 100;
    
    // Primary Stats
    public float hp;
    public float maxHp;
    public float attack;
    public float magic;
    public float defense;
    public float magicDefense;
    public float speed;
    
    // Secondary Stats
    public float critRate = 0.05f;
    public float critDamage = 1.5f;
    public float dodge = 0.03f;
    public float accuracy = 0.95f;
    
    // Equipment
    public List<string> equippedGearIds = new List<string>();
    
    public void LevelUp() {
        level++;
        xp -= xpToNextLevel;
        xpToNextLevel *= 1.25f;
        
        switch (classType) {
            case HeroClass.Knight:
                maxHp += 25;
                defense += 4;
                magicDefense += 2;
                attack += 2;
                break;
            case HeroClass.Ranger:
                attack += 4;
                speed += 3;
                dodge += 0.015f;
                accuracy += 0.01f;
                break;
            case HeroClass.Mage:
                magic += 5;
                magicDefense += 3;
                critDamage += 0.15f;
                break;
        }
        
        CalculatePowerScore();
        hp = maxHp;
    }
    
    public void CalculatePowerScore() {
        // Simple power score calculation
    }
}
