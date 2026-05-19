using UnityEngine;
using System.Collections.Generic;

public class EnhancedGameManager : MonoBehaviour {
    public static EnhancedGameManager Instance { get; private set; }
    public EnhancedHeroData CurrentHero => SaveManager.Instance.CurrentSave.hero as EnhancedHeroData;
    public int CurrentGold => SaveManager.Instance.CurrentSave.gold;
    public int CurrentGems => SaveManager.Instance.CurrentSave.gems;
    
    [Header("Systems")]
    public AchievementSystem achievementSystem;
    public DailyQuestSystem dailyQuestSystem;
    public GuildSystem guildSystem; // Phase 6
    public StatisticsTracker statisticsTracker;
    
    [Header("Settings")]
    public bool enableAutoSave = true;
    public float autoSaveInterval = 30f;
    public bool enablePushNotifications = false; // Enhanced: Placeholder
    
    void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeSystems();
    }
    
    void InitializeSystems() {
        // Enhanced: Initialize all new systems
        if (achievementSystem == null) achievementSystem = gameObject.AddComponent<AchievementSystem>();
        if (dailyQuestSystem == null) dailyQuestSystem = gameObject.AddComponent<DailyQuestSystem>();
        if (statisticsTracker == null) statisticsTracker = gameObject.AddComponent<StatisticsTracker>();
        // Guild system initialized later if unlocked
    }
    
    public void CreateHero(string name, HeroClass classType) {
        var save = SaveManager.Instance.CurrentSave;
        var hero = new EnhancedHeroData {
            heroName = name,
            classType = classType,
            level = 1,
            xp = 0,
            xpToNextLevel = 100,
            combatRating = 0
        };
        
        // Enhanced: Class-specific starting bonuses
        switch (classType) {
            case HeroClass.Knight:
                hero.maxHp = 150; hero.attack = 10; hero.defense = 8;
                hero.magicDefense = 5; hero.speed = 5; hero.magic = 3;
                hero.blockChance = 0.05f;
                break;
            case HeroClass.Ranger:
                hero.maxHp = 100; hero.attack = 15; hero.defense = 5;
                hero.magicDefense = 5; hero.speed = 12; hero.magic = 3;
                hero.dodge += 0.03f; hero.critRate += 0.02f;
                break;
            case HeroClass.Mage:
                hero.maxHp = 80; hero.attack = 5; hero.magic = 20;
                hero.defense = 4; hero.magicDefense = 10; hero.speed = 7;
                hero.lifesteal = 0.02f; hero.critDamage += 0.2f;
                break;
        }
        
        hero.hp = hero.maxHp;
        hero.CalculatePowerScore();
        save.hero = hero;
        SaveManager.Instance.SaveSave();
        
        // Enhanced: Unlock achievements
        achievementSystem?.UnlockAchievement("first_hero_created");
    }
    
    public void AddXp(float xp) {
        if (CurrentHero == null) return;
        CurrentHero.xp += xp;
        while (CurrentHero.xp >= CurrentHero.xpToNextLevel) {
            CurrentHero.LevelUp();
            achievementSystem?.CheckLevelAchievements(CurrentHero.level);
        }
        SaveManager.Instance.SaveSave();
    }
    
    public void AddGold(int amount) {
        SaveManager.Instance.CurrentSave.gold += amount;
        achievementSystem?.CheckGoldAchievements(CurrentHero); // Enhanced: track gold stats
        SaveManager.Instance.SaveSave();
    }
    
    public void AddGems(int amount) {
        SaveManager.Instance.CurrentSave.gems += amount;
        SaveManager.Instance.SaveSave();
    }
    
    public bool SpendGold(int amount) {
        if (CurrentGold < amount) return false;
        SaveManager.Instance.CurrentSave.gold -= amount;
        SaveManager.Instance.SaveSave();
        return true;
    }
    
    public bool SpendGems(int amount) {
        if (CurrentGems < amount) return false;
        SaveManager.Instance.CurrentSave.gems -= amount;
        SaveManager.Instance.SaveSave();
        return true;
    }
    
    // Enhanced: Skill tree system
    public bool UnlockSkillPoint() {
        if (CurrentHero == null) return false;
        if (CurrentHero.skillPoints <= 0) return false;
        // Skill tree logic here
        return true;
    }
    
    // Enhanced: Talent system
    public bool UnlockTalentPoint() {
        if (CurrentHero == null) return false;
        if (CurrentHero.talentPoints <= 0) return false;
        // Talent tree logic here
        return true;
    }
    
    // Daily quests
    public void RefreshDailyQuests() {
        dailyQuestSystem?.GenerateDailyQuests();
    }
    
    // Statistics tracking
    public void RecordBattleResult(bool won, float damageDealt, float damageTaken) {
        if (won) {
            CurrentHero.totalBattlesWon++;
            CurrentHero.totalDamageDealt += damageDealt;
            statisticsTracker?.RecordBattleWon(damageDealt, damageTaken);
        } else {
            CurrentHero.totalBattlesLost++;
            CurrentHero.totalDamageTaken += damageTaken;
            statisticsTracker?.RecordBattleLost();
        }
    }
}
