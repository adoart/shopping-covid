using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    [Header("Level Definitions")]
    [Space]
    public List<LevelDefinition> levels;
    [HideInInspector] public LevelDefinition currentLevel;
    private int currentLevelIndex = 0;

    // Start is called before the first frame update
    void Start() {
        StarLevel(levels[currentLevelIndex]);
    }
    private void StarLevel(LevelDefinition levelDefinition) {
        SpawnPlayer();
        SpawnNPCs(levelDefinition.numberOfNPCs, levelDefinition.numberOfEnemies);
        SpawnItems();
    }
    private void SpawnItems() {
        //Spawn Item/Powerup
        throw new System.NotImplementedException();
    }
    private void SpawnNPCs(int levelDefinitionNumberOfNpCs, int levelDefinitionNumberOfEnemies) {
        //Spawn NPCs/Enemies
        throw new System.NotImplementedException();
    }
    private void SpawnPlayer() {
        //Spawn Player
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
