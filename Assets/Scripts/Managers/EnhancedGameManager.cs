using UnityEngine;

public class EnhancedGameManager : MonoBehaviour {
    public static EnhancedGameManager Instance { get; private set; }
    public EnhancedHeroData CurrentHero => SaveManager.Instance.CurrentSave.hero;
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void CreateHero(string name, HeroClass classType) {
        var save = SaveManager.Instance.CurrentSave;
        save.heroName = name;
        save.hero = new EnhancedHeroData();
        save.hero.heroName = name;
        save.hero.classType = classType;
        save.hero.level = 1;
        
        // Set base stats by class
        switch (classType) {
            case HeroClass.Knight:
                save.hero.maxHp = 150;
                save.hero.attack = 10;
                save.hero.defense = 8;
                save.hero.magicDefense = 5;
                save.hero.speed = 5;
                save.hero.magic = 3;
                break;
            case HeroClass.Ranger:
                save.hero.maxHp = 100;
                save.hero.attack = 15;
                save.hero.defense = 5;
                save.hero.magicDefense = 5;
                save.hero.speed = 12;
                save.hero.magic = 3;
                break;
            case HeroClass.Mage:
                save.hero.maxHp = 80;
                save.hero.attack = 5;
                save.hero.defense = 4;
                save.hero.magicDefense = 10;
                save.hero.speed = 7;
                save.hero.magic = 20;
                break;
        }
        save.hero.hp = save.hero.maxHp;
        SaveManager.Instance.SaveSave();
    }
    
    public void AddXp(float amount) {
        if (CurrentHero == null) return;
        CurrentHero.xp += amount;
        while (CurrentHero.xp >= CurrentHero.xpToNextLevel) {
            CurrentHero.LevelUp();
        }
        SaveManager.Instance.SaveSave();
    }
}
