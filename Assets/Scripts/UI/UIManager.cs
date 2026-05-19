using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {
    public static UIManager Instance { get; private set; }
    
    [Header("Screen Containers")]
    public GameObject heroCreateScreen;
    public GameObject homeScreen;
    public GameObject battleScreen;
    public GameObject gearScreen;
    public GameObject petScreen;
    public GameObject shopScreen;
    public GameObject settingsScreen;
    
    [Header("Battle UI - Mobile Optimized")]
    public Slider heroHpBar;
    public Slider enemyHpBar;
    public Text heroHpText;
    public Text enemyHpText;
    public Text battleLogText;
    public ScrollRect battleLogScrollRect;
    public Text comboText;
    public Text stageText;
    public Button battleSpeedButton;
    public Text battleSpeedText;
    public Button stopBattleButton;
    
    [Header("Home UI")]
    public Text heroNameText;
    public Text heroLevelText;
    public Text goldText;
    public Text gemsText;
    public Text powerScoreText;
    public Image heroPortrait;
    public Button startBattleButton;
    public Button gearButton;
    public Button petsButton;
    public Button shopButton;
    public Button settingsButton;
    
    [Header("Mobile Settings")]
    public CanvasScaler canvasScaler; // For different screen sizes
    public float touchSensitivity = 1.0f;
    public bool enableHapticFeedback = true; // Mobile vibration
    public int targetFPS = 60; // Battery saver
    
    [Header("Audio")]
    public AudioSource uiClickSource;
    
    private List<string> battleLogEntries = new List<string>();
    private const int MAX_LOG_ENTRIES = 8;
    
    void Awake() {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Mobile optimizations
        Application.targetFrameRate = targetFPS;
        if (enableHapticFeedback && Application.isMobilePlatform) {
            // Handled via plugin (simplified here)
        }
    }
    
    void Start() {
        // Force portrait
        Screen.orientation = ScreenOrientation.Portrait;
        
        // Show appropriate screen
        if (string.IsNullOrEmpty(SaveManager.Instance?.CurrentSave?.hero?.heroName)) {
            ShowHeroCreate();
        } else {
            ShowHome();
        }
        
        // Subscribe to events
        if (EnhancedBattleManager.Instance != null) {
            EnhancedBattleManager.Instance.OnBattleStarted += OnBattleStarted;
            EnhancedBattleManager.Instance.OnBattleWon += OnBattleWon;
            EnhancedBattleManager.Instance.OnBattleLost += OnBattleLost;
            EnhancedBattleManager.Instance.OnHeroHpChanged += UpdateHeroHpBar;
            EnhancedBattleManager.Instance.OnEnemyHpChanged += UpdateEnemyHpBar;
            EnhancedBattleManager.Instance.OnBattleLog += AddBattleLog;
            EnhancedBattleManager.Instance.OnComboChanged += UpdateComboText;
            EnhancedBattleManager.Instance.OnStageChanged += UpdateStageText;
            EnhancedBattleManager.Instance.OnBattleSpeedChanged += UpdateBattleSpeedText;
        }
    }
    
    // Screen Management
    public void ShowHeroCreate() {
        SetScreenActive(heroCreateScreen);
    }
    
    public void ShowHome() {
        SetScreenActive(homeScreen);
        UpdateHomeUI();
    }
    
    public void ShowBattle() {
        SetScreenActive(battleScreen);
    }
    
    public void ShowGear() {
        SetScreenActive(gearScreen);
    }
    
    public void ShowPets() {
        SetScreenActive(petScreen);
    }
    
    void SetScreenActive(GameObject screenToShow) {
        heroCreateScreen?.SetActive(false);
        homeScreen?.SetActive(false);
        battleScreen?.SetActive(false);
        gearScreen?.SetActive(false);
        petScreen?.SetActive(false);
        shopScreen?.SetActive(false);
        settingsScreen?.SetActive(false);
        
        if (screenToShow != null) {
            screenToShow.SetActive(true);
        }
    }
    
    // Battle UI Updates
    void UpdateHeroHpBar(float currentHp, float maxHp) {
        if (heroHpBar != null) {
            heroHpBar.value = currentHp / maxHp;
        }
        if (heroHpText != null) {
            heroHpText.text = $"{currentHp:F0}/{maxHp:F0}";
        }
    }
    
    void UpdateEnemyHpBar(float currentHp, float maxHp) {
        if (enemyHpBar != null) {
            enemyHpBar.value = currentHp / maxHp;
        }
        if (enemyHpText != null) {
            enemyHpText.text = $"{currentHp:F0}/{maxHp:F0}";
        }
    }
    
    void AddBattleLog(string message) {
        battleLogEntries.Add(message);
        if (battleLogEntries.Count > MAX_LOG_ENTRIES) {
            battleLogEntries.RemoveAt(0);
        }
        
        if (battleLogText != null) {
            battleLogText.text = string.Join("\n", battleLogEntries);
        }
        
        // Auto-scroll
        if (battleLogScrollRect != null) {
            Canvas.ForceUpdateCanvases();
            battleLogScrollRect.verticalNormalizedPosition = 0f;
        }
    }
    
    void UpdateComboText(int combo) {
        if (comboText != null) {
            comboText.text = combo > 0 ? $"COMBO x{combo}!" : "";
            comboText.color = combo > 10 ? Color.yellow : Color.white;
        }
    }
    
    void UpdateStageText(int stage) {
        if (stageText != null) {
            stageText.text = $"Stage {stage}";
        }
    }
    
    void UpdateBattleSpeedText(float speed) {
        if (battleSpeedText != null) {
            battleSpeedText.text = $"{speed}x";
        }
    }
    
    // Home UI Updates
    void UpdateHomeUI() {
        var hero = EnhancedGameManager.Instance?.CurrentHero;
        if (hero == null) return;
        
        if (heroNameText != null) heroNameText.text = hero.heroName;
        if (heroLevelText != null) heroLevelText.text = $"Lv. {hero.level}";
        if (powerScoreText != null) powerScoreText.text = $"Power: {hero.powerScore:F0}";
        
        if (goldText != null) goldText.text = $"Gold: {EnhancedGameManager.Instance.CurrentGold}";
        if (gemsText != null) gemsText.text = $"Gems: {EnhancedGameManager.Instance.CurrentGems}";
    }
    
    // Button Handlers
    public void OnStartBattleClicked() {
        PlayClickSound();
        ShowBattle();
        EnhancedBattleManager.Instance?.StartBattle(1); // Simplified
    }
    
    public void OnStopBattleClicked() {
        PlayClickSound();
        EnhancedBattleManager.Instance?.StopBattle();
        ShowHome();
    }
    
    public void OnBattleSpeedClicked() {
        PlayClickSound();
        if (EnhancedBattleManager.Instance != null) {
            float newSpeed = EnhancedBattleManager.Instance.BattleSpeed >= 4f ? 1f : EnhancedBattleManager.Instance.BattleSpeed * 2f;
            EnhancedBattleManager.Instance.SetBattleSpeed(newSpeed);
        }
    }
    
    public void OnGearClicked() {
        PlayClickSound();
        ShowGear();
    }
    
    public void OnPetsClicked() {
        PlayClickSound();
        ShowPets();
    }
    
    // Event Handlers
    void OnBattleStarted() {
        ShowBattle();
    }
    
    void OnBattleWon() {
        Invoke("ShowHome", 2f);
    }
    
    void OnBattleLost() {
        Invoke("ShowHome", 2f);
    }
    
    void PlayClickSound() {
        if (uiClickSource != null) {
            uiClickSource.Play();
        }
    }
    
    // Mobile-specific
    void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            // App going to background - save
            SaveManager.Instance?.SaveSave();
        }
    }
}
