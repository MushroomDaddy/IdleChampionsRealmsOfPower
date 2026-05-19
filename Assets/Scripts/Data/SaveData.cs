using System;
using System.Collections.Generic;

[Serializable]
public class SaveData {
    public string heroName;
    public EnhancedHeroData hero;
    public int gold = 0;
    public int gems = 0;
    public int currentStage = 1;
    public long lastLogoutTimestamp;
    
    public SaveData() {
        hero = new EnhancedHeroData();
    }
}
