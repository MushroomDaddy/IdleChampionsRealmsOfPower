using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "IdleChampions/Skill Config")]
public class SkillConfig : ScriptableObject {
    public string skillId;
    public string skillName;
    public float damageMultiplier = 1.0f;
    public int cooldown = 3;
    public bool isPassive = false;
    public string description = "";
}
