using UnityEngine;

[CreateAssetMenu(fileName = "NewStage", menuName = "IdleChampions/Stage Config")]
public class StageConfig : ScriptableObject {
    public int stageNumber;
    public string stageName;
    public bool isBoss = false;
    
    [Header("Enemy Stats")]
    public float enemyMaxHp = 50;
    public float enemyAttack = 5;
    public float enemyDefense = 3;
    public float enemySpeed = 5;
    
    [Header("Rewards")]
    public int goldReward = 10;
    public float xpReward = 5;
}
