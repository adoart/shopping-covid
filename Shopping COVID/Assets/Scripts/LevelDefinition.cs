using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Level Definition", fileName = "NewLevelDefinition")]
public class LevelDefinition : ScriptableObject {
    public string levelName;
    public int numberOfEnemies;
    public int numberOfNPCs;
    public int numberOfMaskPowerUps;
    public int numberOfTrolleyPowerUps;
    [Min(1)]
    public int numberOfItems;
    public int mapWith;
    public int mapHeight;
    public List<GameObject> isleAssets;
}