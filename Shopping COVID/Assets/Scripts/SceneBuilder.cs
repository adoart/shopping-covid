using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class SceneBuilder : MonoBehaviour {
    [FormerlySerializedAs("ISLE_VERT_MARGIN")]
    [SerializeField]
    private float isleVertMargin = 5;
    [FormerlySerializedAs("ISLE_HOR_MARGIN")]
    [SerializeField]
    private float isleHorMargin = 4;

    [SerializeField]
    private LevelDefinition levelDefinition;

    [SerializeField]
    private List<GameObject> floorAssets;
    [SerializeField]
    private List<GameObject> wallAssets;
    [SerializeField]
    private List<GameObject> isleAssets;
    [SerializeField]
    private GameObject startAsset;
    [SerializeField]
    private GameObject exitAsset;
    [SerializeField]
    private GameObject invisibleWall;
    [SerializeField]
    private GameObject exitInvisibleWall;
    [SerializeField]
    private List<GameObject> npcs;
    [SerializeField]
    private List<GameObject> enemies;

    [SerializeField]
    private NavMeshSurface[] surfaces;
    private GameObject exitInstance;
    [SerializeField]
    private bool randomize;

    private int mapHeight;
    private int mapWith;

    // Start is called before the first frame update
    void Start() {
        UpdateMap();
    }

    public void UpdateMap() {
        ClearMap();
        BuildMap();
        foreach (NavMeshSurface surface in surfaces) {
            surface.BuildNavMesh();
        }
    }

    public void ClearMap() {
        if (surfaces != null) {
            foreach (NavMeshSurface surface in surfaces) {
                surface.RemoveData();
            }
        }
        GameObject[] mapAssets = GameObject.FindGameObjectsWithTag("Procedural");
        foreach (GameObject mapAsset in mapAssets) {
            DestroyImmediate(mapAsset);
        }
        if (exitInstance) {
            DestroyImmediate(exitInstance);
        }
    }

    public void BuildMap() {
        mapHeight = levelDefinition.mapHeight;
        mapWith = levelDefinition.mapWith;
        //Layout floor
        LayoutFloor();
        //Layout walls stalls
        LayoutWalls();
        //Layout isle stalls
        LayoutIsles();
        //Layout Start & Exit
        LayoutStartExit();
        //Spawn NPCs
        SpawnNPCs(levelDefinition.numberOfNPCs, npcs);
        SpawnNPCs(levelDefinition.numberOfEnemies, enemies);
    }
    private void LayoutFloor() {
        GameObject floor = floorAssets[Random.Range(0, floorAssets.Count)];
        Vector3 floorSize = floor.GetComponent<BoxCollider>().size;
        for (float i = 0; i <= (mapHeight - floorSize.x); i += floorSize.x) {
            for (float j = 0; j < mapWith; j += floorSize.z) {
                GameObject instance = Instantiate(floor, new Vector3(i, 0, j), floor.transform.rotation);
                instance.tag = "Procedural";
            }
        }
    }
    private void LayoutWalls() {
        GameObject wall = wallAssets[0];
        Vector3 wallSize = wall.GetComponent<BoxCollider>().size;
        //top wall
        for (float j = 0; j < (mapWith - wallSize.x); j += wallSize.x) {
            GameObject instance = Instantiate(wall, new Vector3(-2.75f, 0, j + 1),
                Quaternion.AngleAxis(90, Vector3.up));
            instance.tag = "Procedural";
        }

        //left wall
        for (float i = 0; i < (mapHeight - wallSize.x); i += wallSize.x) {
            GameObject instance = Instantiate(wall, new Vector3(i + 5, 0, 1),
                wall.transform.rotation);
            instance.tag = "Procedural";
        }

        //right wall
        for (float i = 0; i < (mapHeight - wallSize.x); i += wallSize.x) {
            GameObject instance = Instantiate(wall, new Vector3(i - 2.75f, 0, (mapWith) - 1),
                Quaternion.AngleAxis(180, Vector3.up));
            instance.tag = "Procedural";
        }

        //bottom wall
        Vector3 invisibleWallSize = invisibleWall.GetComponent<BoxCollider>().size;
        for (float j = 0;
            j < (mapWith - invisibleWallSize.x * 2);
            j += invisibleWallSize.x) { //leave one blank space to place the "Exit" wall
            GameObject instance =
                Instantiate(invisibleWall, new Vector3(mapHeight - 4, 2, j + 4),
                    invisibleWall.transform.rotation);
            instance.tag = "Procedural";
        }

        //Exit wall
        exitInstance = Instantiate(exitInvisibleWall,
            new Vector3(mapHeight - 5.5f, 0, mapWith - 11), exitInvisibleWall.transform.rotation);
        exitInstance.tag = "Exit";
    }
    private void LayoutIsles() {
        GameObject isle = isleAssets[0 /*Random.Range(0, isleAssets.Count)*/];
        Vector3 isleSize = isle.GetComponent<BoxCollider>().size;
        for (float i = isleVertMargin;
            i < mapHeight - (isleSize.x + isleVertMargin);
            i += isleSize.x + isleVertMargin) {
            for (float j = isleHorMargin; j < mapWith - (isleSize.z + isleHorMargin); j += isleSize.z + isleHorMargin) {
                GameObject instance = Instantiate(isle, new Vector3(i, 0, j), isle.transform.rotation);
                instance.tag = "Procedural";
                if (randomize) {
                    isle = isleAssets[Random.Range(0, isleAssets.Count)]; //not a good idea with different sizes...
                    isleSize = isle.GetComponent<BoxCollider>().size;
                }
            }
        }
    }
    private void LayoutStartExit() {
        //Start on bottom left (0 x levelDefinitionMapHeight)
        GameObject start = Instantiate(startAsset, new Vector3(mapHeight - 5.5f, 0, 7),
            startAsset.transform.rotation);
        start.tag = "Procedural";

        //Exit on bottom right (levelDefinitionMapWith x levelDefinitionMapHeight)
        GameObject exit = Instantiate(exitAsset, new Vector3(mapHeight - 6.5f, 0, mapWith - 13),
            exitAsset.transform.rotation);
        exit.tag = "Procedural";

    }

    private void SpawnNPCs(int quantity, List<GameObject> prefabsList) {
        for (int i = 0; i < quantity; i++) {
            GameObject npcPrefab = prefabsList[Random.Range(0, prefabsList.Count)];
            Vector3 position = GetNPCRandomPosition();
            GameObject instance = Instantiate(npcPrefab, position, npcPrefab.transform.rotation);
            instance.tag = "Procedural";

            // if spawn inside something try to move it out
            int maxTries = 10;
            Collider[] colliders = new Collider[5];
            while (Physics.OverlapSphereNonAlloc(instance.transform.position, 1.0f, colliders) > 1 && maxTries > 0) {
                instance.transform.position = GetNPCRandomPosition();
                maxTries--;
            }
        }
    }
    private Vector3 GetNPCRandomPosition() {
        return new Vector3(Random.Range(isleVertMargin, mapHeight - isleVertMargin), 1,
            Random.Range(isleHorMargin, mapWith - isleHorMargin));
    }
}