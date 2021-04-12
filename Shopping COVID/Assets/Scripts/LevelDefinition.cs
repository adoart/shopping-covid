using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Level Definition", fileName = "NewLevelDefinition")]
public class LevelDefinition : ScriptableObject {
    public string levelName;
    public int numberOfEnemies;
    public int numberOfNPCs;
    public int numberOfMaskPowerUps;
    public int numberOfTrolleyPowerUps;
    public int mapWith;
    public int mapHeight;
}