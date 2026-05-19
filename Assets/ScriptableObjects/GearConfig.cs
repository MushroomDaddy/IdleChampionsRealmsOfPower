using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGear", menuName = "IdleChampions/Gear Config")]
public class GearConfig : ScriptableObject {
    [Header("Identity")]
    public string gearId;
    public string gearName;
    public GearSlot slot;
    public GearRarity rarity;
    
    [Header("Stats")]
    public float attackBonus = 0;
    public float defenseBonus = 0;
    public float hpBonus = 0;
    public float speedBonus = 0;
    
    [Header("Enhancement")]
    [Range(0, 15)] public int enhanceLevel = 0;
    public int maxEnhanceLevel = 15;
    
    [Header("Other")]
    public int sellValue = 10;
    public string setName = "";
}
