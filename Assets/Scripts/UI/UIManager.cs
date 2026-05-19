using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }
    
    [SerializeField] private GameObject heroCreateScreen;
    [SerializeField] private GameObject homeScreen;
    [SerializeField] private GameObject battleScreen;
    
    void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    void Start() {
        if (string.IsNullOrEmpty(SaveManager.Instance.CurrentSave.heroName)) {
            ShowHeroCreate();
        } else {
            ShowHome();
        }
    }
    
    public void ShowHeroCreate() {
        heroCreateScreen?.SetActive(true);
        homeScreen?.SetActive(false);
        battleScreen?.SetActive(false);
    }
    
    public void ShowHome() {
        heroCreateScreen?.SetActive(false);
        homeScreen?.SetActive(true);
        battleScreen?.SetActive(false);
    }
    
    public void ShowBattle() {
        heroCreateScreen?.SetActive(false);
        homeScreen?.SetActive(false);
        battleScreen?.SetActive(true);
    }
    
    public void OnCreateHero(string name, int classIndex) {
        HeroClass cls = (HeroClass)classIndex;
        EnhancedGameManager.Instance.CreateHero(name, cls);
        ShowHome();
    }
    
    public void OnStartBattleClicked() {
        ShowBattle();
        EnhancedBattleManager.Instance.StartBattle(SaveManager.Instance.CurrentSave.currentStage);
    }
    
    public void OnStopBattleClicked() {
        EnhancedBattleManager.Instance.StopBattle();
        ShowHome();
    }
}
