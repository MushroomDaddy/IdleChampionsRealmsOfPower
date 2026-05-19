using System.Collections;
using UnityEngine;

public class EnhancedBattleManager : MonoBehaviour {
    public static EnhancedBattleManager Instance { get; private set; }
    public bool IsBattling { get; private set; }
    public int CurrentStage { get; private set; } = 1;
    
    // Events for UI
    public event System.Action OnBattleStarted;
    public event System.Action<float, float> OnDamageDealt;
    public event System.Action OnBattleWon;
    public event System.Action OnBattleLost;
    public event System.Action<float, float> OnHeroHpChanged;
    public event System.Action<float, float> OnEnemyHpChanged;
    
    private EnhancedHeroData hero;
    private float enemyHp;
    private float enemyMaxHp;
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void StartBattle(int stage) {
        if (IsBattling) return;
        CurrentStage = stage;
        hero = EnhancedGameManager.Instance.CurrentHero;
        if (hero == null) return;
        
        // Simple enemy stats based on stage
        enemyMaxHp = 50 + stage * 15;
        enemyHp = enemyMaxHp;
        
        IsBattling = true;
        OnBattleStarted?.Invoke();
        OnEnemyHpChanged?.Invoke(enemyHp, enemyMaxHp);
        OnHeroHpChanged?.Invoke(hero.hp, hero.maxHp);
        
        StartCoroutine(BattleLoop());
    }
    
    IEnumerator BattleLoop() {
        while (IsBattling && hero.hp > 0 && enemyHp > 0) {
            // Hero attacks
            float heroDmg = hero.attack - (stage * 0.5f);
            if (heroDmg < 1) heroDmg = 1;
            enemyHp -= heroDmg;
            if (enemyHp < 0) enemyHp = 0;
            OnEnemyHpChanged?.Invoke(enemyHp, enemyMaxHp);
            OnDamageDealt?.Invoke(heroDmg, enemyHp);
            
            yield return new WaitForSeconds(1f);
            
            if (enemyHp <= 0) break;
            
            // Enemy attacks
            float enemyDmg = (5 + CurrentStage * 3) - (hero.defense * 0.5f);
            if (enemyDmg < 1) enemyDmg = 1;
            hero.hp -= enemyDmg;
            if (hero.hp < 0) hero.hp = 0;
            OnHeroHpChanged?.Invoke(hero.hp, hero.maxHp);
            
            yield return new WaitForSeconds(1f);
        }
        
        IsBattling = false;
        
        if (hero.hp <= 0) {
            OnBattleLost?.Invoke();
        } else {
            OnBattleWon?.Invoke();
        }
    }
    
    public void StopBattle() {
        IsBattling = false;
        StopAllCoroutines();
    }
}
