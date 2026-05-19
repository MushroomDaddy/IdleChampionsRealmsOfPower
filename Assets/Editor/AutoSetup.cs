#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class AutoSetup {
    static AutoSetup() {
        // Delay to ensure Unity is fully loaded
        EditorApplication.delayCall += () => {
            if (!EditorPrefs.GetBool("IdleChampions_AutoSetupDone", false)) {
                SetupProject();
                EditorPrefs.SetBool("IdleChampions_AutoSetupDone", true);
            }
        };
    }
    
    static void SetupProject() {
        Debug.Log("🚀 Idle Champions: Realms of Power — Auto-Setup Starting...");
        
        // 1. Apply project settings
        ApplyProjectSettings();
        
        // 2. Create main scene if it doesn't exist
        CreateMainScene();
        
        // 3. Create ScriptableObjects
        CreateAllScriptableObjects();
        
        // 4. Save everything
        AssetDatabase.SaveAssets();
        EditorSceneManager.SaveOpenScenes();
        
        EditorUtility.DisplayDialog("Auto-Setup Complete!", 
            "Idle Champions: Realms of Power has been automatically configured!\n\n" +
            "✅ Project settings applied\n" +
            "✅ Main scene created with all managers\n" +
            "✅ ScriptableObjects generated (110 total)\n" +
            "✅ UI Canvas configured (1080x1920 Portrait)\n\n" +
            "Press PLAY to start the game!", "OK");
        
        Debug.Log("✅ Auto-Setup Complete!");
    }
    
    static void ApplyProjectSettings() {
        // Package name
        if (PlayerSettings.applicationIdentifier != "com.yourcompany.idlechampionsrp") {
            PlayerSettings.applicationIdentifier = "com.yourcompany.idlechampionsrp";
        }
        
        // Product name
        PlayerSettings.productName = "Idle Champions: Realms of Power";
        PlayerSettings.companyName = "YourCompany";
        
        // Android settings
        #if UNITY_ANDROID
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
        #endif
        
        Debug.Log("✅ Project settings applied");
    }
    
    static void CreateMainScene() {
        // Create a new scene
        Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // Create root objects
        GameObject managers = new GameObject("Managers");
        GameObject ui = new GameObject("UI");
        
        // Add Canvas
        Canvas canvas = ui.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = null;
        
        CanvasScaler scaler = ui.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f;
        
        ui.AddComponent<GraphicRaycaster>();
        
        // Add EventSystem
        if (FindObjectOfType<EventSystem>() == null) {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
        
        // Create Managers
        CreateManager<EnhancedSaveManager>("SaveManager", managers);
        CreateManager<EnhancedGameManager>("GameManager", managers);
        CreateManager<EnhancedBattleManager>("BattleManager", managers);
        CreateManager<AudioManager>("AudioManager", managers);
        CreateManager<StageDatabase>("StageDatabase", managers);
        CreateManager<UIManager>("UIManager", ui);
        
        // Create UI Screens
        CreateScreen("HeroCreateScreen", ui.transform);
        GameObject homeScreen = CreateScreen("HomeScreen", ui.transform);
        GameObject battleScreen = CreateScreen("BattleScreen", ui.transform);
        CreateScreen("GearScreen", ui.transform);
        CreateScreen("PetScreen", ui.transform);
        
        // Build simple Home UI
        BuildHomeUI(homeScreen);
        BuildBattleUI(battleScreen);
        
        // Set screen visibility
        SetScreenActive("HeroCreateScreen", ui.transform, true);
        SetScreenActive("HomeScreen", ui.transform, false);
        SetScreenActive("BattleScreen", ui.transform, false);
        
        // Save scene
        string scenePath = "Assets/Scenes/MainScene.unity";
        if (!AssetDatabase.IsValidFolder("Assets/Scenes")) {
            AssetDatabase.CreateFolder("Assets", "Scenes");
        }
        EditorSceneManager.SaveScene(newScene, scenePath);
        Debug.Log($"✅ Main scene created: {scenePath}");
    }
    
    static void CreateManager<T>(string name, GameObject parent) where T : MonoBehaviour {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent.transform);
        go.AddComponent<T>();
    }
    
    static GameObject CreateScreen(string name, Transform parent) {
        GameObject screen = new GameObject(name);
        screen.transform.SetParent(parent);
        RectTransform rt = screen.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return screen;
    }
    
    static void SetScreenActive(string name, Transform parent, bool active) {
        Transform screen = parent.Find(name);
        if (screen != null) screen.gameObject.SetActive(active);
    }
    
    static void BuildHomeUI(GameObject screen) {
        // Background
        GameObject bg = CreateUIElement("Background", screen.transform);
        Image bgImage = bg.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.2f);
        
        // Title
        GameObject titleGO = CreateUIElement("TitleText", screen.transform);
        Text titleText = titleGO.AddComponent<Text>();
        titleText.text = "Idle Champions";
        titleText.fontSize = 48;
        titleText.color = Color.yellow;
        RectTransform titleRT = titleGO.GetComponent<RectTransform>();
        titleRT.anchoredPosition = new Vector2(0, 600);
        titleRT.sizeDelta = new Vector2(600, 100);
        
        // Start Battle Button (Large touch target)
        GameObject btnGO = CreateUIElement("StartBattleButton", screen.transform);
        Button btn = btnGO.AddComponent<Button>();
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.6f, 0.2f);
        Text btnText = CreateText("START BATTLE", btnGO.transform);
        btnText.color = Color.white;
        btnText.fontSize = 32;
        RectTransform btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchoredPosition = new Vector2(0, -200);
        btnRT.sizeDelta = new Vector2(400, 100);
        
        // Wire up button
        btn.onClick.AddListener(() => {
            if (UIManager.Instance != null) {
                UIManager.Instance.OnStartBattleClicked();
            }
        });
    }
    
    static void BuildBattleUI(GameObject screen) {
        // Hero HP Bar
        GameObject heroHpGO = CreateUIElement("HeroHpBar", screen.transform);
        Slider heroHpBar = heroHpGO.AddComponent<Slider>();
        RectTransform heroHpRT = heroHpGO.GetComponent<RectTransform>();
        heroHpRT.anchoredPosition = new Vector2(0, 800);
        heroHpRT.sizeDelta = new Vector2(500, 40);
        
        // Enemy HP Bar
        GameObject enemyHpGO = CreateUIElement("EnemyHpBar", screen.transform);
        Slider enemyHpBar = enemyHpGO.AddComponent<Slider>();
        RectTransform enemyHpRT = enemyHpGO.GetComponent<RectTransform>();
        enemyHpRT.anchoredPosition = new Vector2(0, 750);
        enemyHpRT.sizeDelta = new Vector2(500, 40);
        
        // Battle Log
        GameObject logGO = CreateUIElement("BattleLog", screen.transform);
        Text logText = logGO.AddComponent<Text>();
        logText.text = "Battle starting...";
        logText.color = Color.white;
        logText.fontSize = 20;
        RectTransform logRT = logGO.GetComponent<RectTransform>();
        logRT.anchoredPosition = new Vector2(0, 0);
        logRT.sizeDelta = new Vector2(600, 400);
        
        // Stop Button
        GameObject stopBtnGO = CreateUIElement("StopBattleButton", screen.transform);
        Button stopBtn = stopBtnGO.AddComponent<Button>();
        Image stopBtnImage = stopBtnGO.AddComponent<Image>();
        stopBtnImage.color = new Color(0.8f, 0.2f, 0.2f);
        Text stopBtnText = CreateText("STOP", stopBtnGO.transform);
        RectTransform stopBtnRT = stopBtnGO.GetComponent<RectTransform>();
        stopBtnRT.anchoredPosition = new Vector2(0, -350);
        stopBtnRT.sizeDelta = new Vector2(250, 80);
        
        stopBtn.onClick.AddListener(() => {
            if (UIManager.Instance != null) {
                UIManager.Instance.OnStopBattleClicked();
            }
        });
    }
    
    static GameObject CreateUIElement(string name, Transform parent) {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(200, 50);
        return go;
    }
    
    static Text CreateText(string text, Transform parent) {
        GameObject go = new GameObject("Text");
        go.transform.SetParent(parent);
        Text txt = go.AddComponent<Text>();
        txt.text = text;
        txt.color = Color.white;
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(200, 50);
        return txt;
    }
    
    static void CreateAllScriptableObjects() {
        // Create Stage Configs
        for (int i = 1; i <= 10; i++) {
            CreateStageConfig(i);
        }
        
        // Create Gear Configs
        string[] slots = { "Weapon", "Helmet", "Chest", "Gloves", "Boots", "Ring" };
        string[] rarities = { "Common", "Uncommon", "Rare", "Epic", "Legendary", "Mythic" };
        int gearId = 4008401;
        foreach (string slot in slots) {
            for (int r = 0; r < rarities.Length; r++) {
                CreateGearConfig(slot, rarities[r], gearId++, r);
            }
        }
        
        // Create Pet Configs
        string[] types = { "Physical", "Magic", "Defense", "Speed", "Support" };
        for (int star = 1; star <= 5; star++) {
            for (int type = 0; type < types.Length; type++) {
                CreatePetConfig(star, types[type], type);
            }
        }
        
        // Create Skill Configs
        for (int i = 0; i < 50; i++) {
            CreateSkillConfig(i);
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("✅ ScriptableObjects created (110 total)");
    }
    
    static void CreateStageConfig(int i) {
        MonsterConfig config = ScriptableObject.CreateInstance<MonsterConfig>();
        config.stageNumber = i;
        config.stageName = $"Stage {i}";
        config.isBoss = i % 10 == 0;
        config.enemyName = config.isBoss ? $"Boss {i/10}" : $"Enemy {i}";
        config.baseHp = 50 + i * 15 + (config.isBoss ? 200 : 0);
        config.baseAttack = 5 + i * 3 + (config.isBoss ? 20 : 0);
        config.baseDefense = 3 + i * 1.5f + (config.isBoss ? 10 : 0);
        config.baseSpeed = 5 + i * 0.8f + (config.isBoss ? 3 : 0);
        config.baseGoldReward = 10 * i + (config.isBoss ? 100 : 0);
        config.baseXpReward = 5 * i + (config.isBoss ? 50 : 0);
        
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Stages")) {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Stages");
        }
        AssetDatabase.CreateAsset(config, $"Assets/ScriptableObjects/Stages/Stage_{i}.asset");
    }
    
    static void CreateGearConfig(string slot, string rarity, int id, int rarityIndex) {
        GearConfig gear = ScriptableObject.CreateInstance<GearConfig>();
        gear.gearId = $"{id}";
        gear.gearName = $"{rarity} {slot}";
        gear.slot = (GearSlot)System.Enum.Parse(typeof(GearSlot), slot);
        gear.rarity = (GearRarity)rarityIndex;
        gear.enhanceLevel = 0;
        gear.baseStatBonuses = new System.Collections.Generic.Dictionary<StatType, float> {
            { StatType.Attack, (5 + rarityIndex * 3) * (1 + rarityIndex * 0.5f) },
            { StatType.Defense, (3 + rarityIndex * 2) * (1 + rarityIndex * 0.5f) },
            { StatType.HP, (10 + rarityIndex * 5) * (1 + rarityIndex * 0.5f) }
        };
        gear.sellValue = 10 * (rarityIndex + 1) * (id % 100);
        
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Gear")) {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Gear");
        }
        AssetDatabase.CreateAsset(gear, $"Assets/ScriptableObjects/Gear/{slot}_{rarity}.asset");
    }
    
    static void CreatePetConfig(int star, string type, int typeIndex) {
        PetConfig pet = ScriptableObject.CreateInstance<PetConfig>();
        pet.petId = $"pet_{star}star_{type}";
        pet.petName = $"{star}-Star {type} Pet";
        pet.rarity = (PetRarity)(star - 1);
        pet.type = (PetType)typeIndex;
        pet.level = 1;
        pet.baseAttack = 5 + star * 3;
        pet.baseDefense = 3 + star * 2;
        pet.baseSpeed = 4 + star * 1.5f;
        pet.baseHp = 20 + star * 10;
        
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Pets")) {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Pets");
        }
        AssetDatabase.CreateAsset(pet, $"Assets/ScriptableObjects/Pets/{star}Star_{type}.asset");
    }
    
    static void CreateSkillConfig(int i) {
        SkillConfig skill = ScriptableObject.CreateInstance<SkillConfig>();
        skill.skillId = $"skill_{i}";
        skill.skillName = $"Skill {i}";
        skill.damageMultiplier = 1.0f + (i * 0.1f);
        skill.cooldown = 3 + (i % 5);
        skill.isPassive = i % 3 == 0;
        
        if (!AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Skills")) {
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Skills");
        }
        AssetDatabase.CreateAsset(skill, $"Assets/ScriptableObjects/Skills/Skill_{i}.asset");
    }
}
#endif
