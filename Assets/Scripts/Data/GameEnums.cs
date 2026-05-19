using UnityEngine;

public enum HeroClass {
    Knight,
    Ranger,
    Mage
}

public enum GearSlot {
    Weapon,
    Helmet,
    Chest,
    Gloves,
    Boots,
    Ring
}

public enum GearRarity {
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythic
}

public enum PetRarity {
    OneStar,
    TwoStar,
    ThreeStar,
    FourStar,
    FiveStar
}

public enum PetType {
    Physical,
    Magic,
    Defense,
    Speed,
    Support
}

public enum StatType {
    HP,
    MaxHP,
    Attack,
    Magic,
    Defense,
    MagicDefense,
    Speed,
    CritRate,
    CritDamage,
    Dodge,
    Accuracy,
    Block,
    Lifesteal,
    Thorns,
    Tenacity
}

public enum BattleSpeed {
    Normal = 1,
    Fast = 2,
    UltraFast = 4
}

public enum SkillType {
    Active,
    Passive,
    Ultimate
}

public enum ElementType {
    None,
    Fire,
    Water,
    Earth,
    Air,
    Light,
    Dark
}
