using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnhancedBattleManager : MonoBehaviour {
    public static EnhancedBattleManager Instance { get; private set; }
    public int CurrentStage { get; private set; } = 1;
    public bool IsBattling { get; private set; }
    public float BattleSpeed { get; private set; } = 1f; // 1x, 2x, 4x
    public int ComboCounter { get; private set; } = 0; // Enhanced: Combo system
    public float ComboTimer { get; private set; } = 0f;
    public const float COMBO_TIMEOUT = 3f; // Seconds to maintain combo
    
    private Coroutine battleCoroutine;
    private StageDatabase stageDatabase;
    private AudioManager audioManager;
    
    [Header("Events - Enhanced")]
    public event Action OnBattleStarted;
    public event Action<MonsterConfig> OnEnemySpawned;
    public event Action<float, float, bool, bool> OnDamageDealt; // damage, targetHP, isCrit, isMiss
    public event Action<float, float> OnHeroHpChanged;
    public event Action<float, float> OnEnemyHpChanged;
    public event Action OnBattleWon;
    public event Action OnBattleLost;
    public event Action<int, int, int> OnRewardsGranted; // gold, xp, stage
    public event Action<int> OnStageChanged;
    public event Action<int> OnComboChanged; // Enhanced: Combo counter
    public event Action<float> OnBattleSpeedChanged; // Enhanced: Speed control
    public event Action<string> OnBattleLog; // Enhanced: Colored log
    public event Action OnWaveStart; // Enhanced: Wave-based battles
    public event Action OnBossEnraged; // Enhanced: Boss mechanics
    
    [Serializable]
    private class EnhancedEnemyData {
        public string name;
        public float hp;
        public float maxHp;
        public float attack;
        public float defense;
        public float speed;
        public bool isBoss;
        public bool hasEnraged;
        public float enrageThreshold;
        public float enrageMultiplier;
        public int currentWave;
        public int totalWaves;
    }
    
    void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        stageDatabase = FindObjectOfType<StageDatabase>();
        audioManager = AudioManager.Instance;
    }
    
    public void StartBattle(int stage) {
        if (IsBattling) return;
        CurrentStage = stage;
        IsBattling = true;
        ComboCounter = 0;
        battleCoroutine = StartCoroutine(BattleLoop());
    }
    
    public void StopBattle() {
        if (!IsBattling) return;
        IsBattling = false;
        if (battleCoroutine != null) StopCoroutine(battleCoroutine);
        battleCoroutine = null;
    }
    
    public void SetBattleSpeed(float speed) {
        BattleSpeed = Mathf.Clamp(speed, 1f, 4f);
        OnBattleSpeedChanged?.Invoke(BattleSpeed);
        OnBattleLog?.Invoke($"Battle speed: {BattleSpeed}x");
    }
    
    private IEnumerator BattleLoop() {
        OnBattleStarted?.Invoke();
        var save = SaveManager.Instance.CurrentSave;
        var hero = GameManager.Instance.CurrentHero as EnhancedHeroData;
        
        int currentWave = 0;
        int totalWaves = CurrentStage % 10 == 0 ? 3 : 1; // Boss stages have 3 waves
        
        while (IsBattling && currentWave < totalWaves) {
            OnWaveStart?.Invoke();
            currentWave++;
            OnBattleLog?.Invoke($"--- Wave {currentWave}/{totalWaves} ---");
            
            // Spawn enemy (or multiple for waves)
            MonsterConfig stageConfig = stageDatabase != null ? stageDatabase.GetStage(CurrentStage) : null;
            EnhancedEnemyData enemy = CreateEnhancedEnemy(stageConfig, currentWave, totalWaves);
            
            OnEnemySpawned?.Invoke(stageConfig);
            OnEnemyHpChanged?.Invoke(enemy.hp, enemy.maxHp);
            OnStageChanged?.Invoke(CurrentStage);
            
            bool playerFirst = hero.speed >= enemy.speed;
            
            while (enemy.hp > 0 && hero.hp > 0 && IsBattling) {
                // Combo timer
                if (ComboCounter > 0) {
                    ComboTimer += Time.deltaTime * BattleSpeed;
                    if (ComboTimer >= COMBO_TIMEOUT) {
                        ComboCounter = 0;
                        OnComboChanged?.Invoke(ComboCounter);
                        OnBattleLog?.Invoke("Combo reset!");
                    }
                }
                
                if (playerFirst) {
                    yield return StartCoroutine(HeroAttack(hero, enemy));
                    if (enemy.hp <= 0) break;
                    yield return StartCoroutine(EnemyAttack(enemy, hero));
                } else {
                    yield return StartCoroutine(EnemyAttack(enemy, hero));
                    if (hero.hp <= 0) break;
                    yield return StartCoroutine(HeroAttack(hero, enemy));
                }
                
                yield return new WaitForSeconds(1f / BattleSpeed);
            }
            
            if (hero.hp <= 0) {
                OnBattleLost?.Invoke();
                IsBattling = false;
                yield break;
            }
            
            if (currentWave < totalWaves) {
                OnBattleLog?.Invoke($"Wave {currentWave} cleared! Next wave incoming...");
                yield return new WaitForSeconds(2f / BattleSpeed);
            }
        }
        
        // Victory!
        OnBattleWon?.Invoke();
        GrantRewards(hero, save);
        SaveManager.Instance.SaveSave();
        
        // Auto-continue if auto-repeat enabled
        if (IsBattling) {
            CurrentStage++;
            yield return new WaitForSeconds(2f / BattleSpeed);
            // Restart loop for next stage (simplified)
        }
    }
    
    private IEnumerator HeroAttack(EnhancedHeroData hero, EnhancedEnemyData enemy) {
        var (damage, isCrit, isMiss) = hero.DealDamage(false);
        
        if (isMiss) {
            OnBattleLog?.Invoke("<color=yellow>MISS!</color>");
            OnDamageDealt?.Invoke(0, enemy.hp, false, true);
            audioManager?.PlaySFX("miss");
        } else {
            float actualDamage = enemy.hp - damage; // Simplified
            enemy.hp = Mathf.Max(0, enemy.hp - damage);
            
            // Combo system
            ComboCounter++;
            ComboTimer = 0f;
            OnComboChanged?.Invoke(ComboCounter);
            
            // Crit visual
            if (isCrit) {
                OnBattleLog?.Invoke($"<color=red>CRITICAL! {damage:F0} damage</color>");
                audioManager?.PlaySFX("crit");
            } else {
                OnBattleLog?.Invoke($"Hit! {damage:F0} damage");
                audioManager?.PlaySFX("hit");
            }
            
            // Lifesteal
            if (hero.lifesteal > 0) {
                float heal = damage * hero.lifesteal;
                hero.hp = Mathf.Min(hero.maxHp, hero.hp + heal);
                OnBattleLog?.Invoke($"<color=green>Lifesteal: +{heal:F0} HP</color>");
            }
            
            // Thorns damage to enemy? (thorns is for when enemy hits you)
            OnDamageDealt?.Invoke(damage, enemy.hp, isCrit, false);
            OnEnemyHpChanged?.Invoke(enemy.hp, enemy.maxHp);
        }
        
        yield return null;
    }
    
    private IEnumerator EnemyAttack(EnhancedEnemyData enemy, EnhancedHeroData hero) {
        // Enemy attack logic (simplified)
        float damage = Mathf.Max(1, enemy.attack - (hero.defense * 0.5f));
        
        // Boss enrage check
        if (enemy.isBoss && !enemy.hasEnraged && enemy.hp <= enemy.maxHp * enemy.enrageThreshold) {
            enemy.hasEnraged = true;
            enemy.attack *= enemy.enrageMultiplier;
            OnBossEnraged?.Invoke();
            OnBattleLog?.Invoke("<color=purple>Boss enraged! Attack increased!</color>");
            audioManager?.PlaySFX("enrage");
        }
        
        float actualDamage = hero.TakeDamage(damage, false, false);
        if (actualDamage < 0) {
            // Blocked
            OnBattleLog?.Invoke($"<color=blue>BLOCKED! Reduced damage</color>");
            audioManager?.PlaySFX("block");
        } else if (actualDamage == 0) {
            OnBattleLog?.Invoke("<color=yellow>DODGED!</color>");
            audioManager?.PlaySFX("dodge");
        } else {
            OnBattleLog?.Invoke($"Enemy hits for {actualDamage:F0} damage");
            audioManager?.PlaySFX("enemy_hit");
        }
        
        OnHeroHpChanged?.Invoke(hero.hp, hero.maxHp);
        yield return null;
    }
    
    private EnhancedEnemyData CreateEnhancedEnemy(MonsterConfig config, int wave, int totalWaves) {
        // Simplified creation
        var enemy = new EnhancedEnemyData {
            name = config?.monsterName ?? $"Stage {CurrentStage} Enemy",
            hp = config != null ? config.baseHp + (config.hpPerStage * CurrentStage) : 50 + CurrentStage * 10,
            maxHp = config != null ? config.baseHp + (config.hpPerStage * CurrentStage) : 50 + CurrentStage * 10,
            attack = config != null ? config.baseAttack + (config.attackPerStage * CurrentStage) : 5 + CurrentStage * 2,
            defense = config != null ? config.baseDefense + (config.defensePerStage * CurrentStage) : 3 + CurrentStage * 1,
            speed = config != null ? config.baseSpeed + (config.speedPerStage * CurrentStage) : 5 + CurrentStage * 0.5f,
            isBoss = config?.isBoss ?? false,
            hasEnraged = false,
            enrageThreshold = config?.enrageHpThreshold ?? 0.3f,
            enrageMultiplier = config?.enrageDamageMultiplier ?? 1.5f,
            currentWave = wave,
            totalWaves = totalWaves
        };
        return enemy;
    }
    
    private void GrantRewards(EnhancedHeroData hero, SaveData save) {
        int goldReward = 10 * CurrentStage;
        float xpReward = 5 * CurrentStage;
        
        // Combo bonus
        if (ComboCounter > 10) {
            goldReward = (int)(goldReward * 1.5f);
            OnBattleLog?.Invoke($"<color=gold>Combo Bonus! Gold x1.5</color>");
        }
        
        save.gold += goldReward;
        GameManager.Instance.AddXp(xpReward);
        
        if (CurrentStage > save.highestStage) save.highestStage = CurrentStage;
        
        hero.hp = hero.maxHp; // Full heal after victory
        OnHeroHpChanged?.Invoke(hero.hp, hero.maxHp);
        
        OnRewardsGranted?.Invoke(goldReward, (int)xpReward, CurrentStage);
        OnBattleLog?.Invoke($"<color=green>Victory! +{goldReward} Gold, +{xpReward} XP</color>");
    }
}
