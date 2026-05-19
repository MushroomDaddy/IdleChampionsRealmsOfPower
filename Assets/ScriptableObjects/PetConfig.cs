using UnityEngine;

[CreateAssetMenu(fileName = "NewPet", menuName = "IdleChampions/Pet Config")]
public class PetConfig : ScriptableObject {
    public string petId;
    public string petName;
    public PetRarity rarity;
    public PetType type;
    public int level = 1;
    
    public float baseAttack = 5;
    public float baseDefense = 3;
    public float baseSpeed = 4;
    public float baseHp = 20;
    
    public string skillDescription = "Pet skill";
}
