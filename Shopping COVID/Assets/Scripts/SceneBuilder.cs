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

    [Space]
    [Header("Interior")]
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
    private List<GameObject> masks;
    [SerializeField]
    private List<GameObject> trolleys;

    private const int EXTERIOR_MARGIN = 50;
    [Space]
    [Header("Exterior")]
    [SerializeField]
    private List<GameObject> sidewalks;
    [SerializeField]
    private List<GameObject> roads;


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
        GameObject[] npcAssets = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject npcAsset in npcAssets) {
            DestroyImmediate(npcAsset);
        }
        GameObject[] enemyAssets = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyAsset in enemyAssets) {
            DestroyImmediate(enemyAsset);
        }
        GameObject[] maskAssets = GameObject.FindGameObjectsWithTag("Mask");
        foreach (GameObject maskAsset in maskAssets) {
            DestroyImmediate(maskAsset);
        }
        GameObject[] trolleyAssets = GameObject.FindGameObjectsWithTag("Trolley");
        foreach (GameObject trolleyAsset in trolleyAssets) {
            DestroyImmediate(trolleyAsset);
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
        LayoutStartExitPlayer();
        //Spawn NPCs
        SpawnNPCs(levelDefinition.numberOfNPCs, npcs, "NPC");
        SpawnNPCs(levelDefinition.numberOfEnemies, enemies, "Enemy");
        //Spawn Powerups
        SpawnPowerups(levelDefinition.numberOfMaskPowerUps, masks, "Mask");
        SpawnPowerups(levelDefinition.numberOfTrolleyPowerUps, trolleys, "Trolley");
        LayoutPlayer();

        //Layout Exterior
        //Layout Sidewalks
        LayoutSidewalk();
        //Layout Road
        LayoutRoad();
    }
    private void LayoutFloor() {
        GameObject floor = floorAssets[Random.Range(0, floorAssets.Count)];
        Vector3 floorSize = new Vector3(5, 0, 5); //TODO floor tiles are always 5x5 
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
        GameObject isle = isleAssets[1];
        Vector3 isleSize = isle.GetComponent<BoxCollider>().size;
        for (float i = isleVertMargin;
            i < mapHeight - (isleSize.x + isleVertMargin);
            i += isleSize.x + isleVertMargin) {
            for (float j = isleHorMargin;
                j < mapWith - ( /*isleSize.z +*/ isleHorMargin);
                j += isleSize.z + isleHorMargin) {
                GameObject instance = Instantiate(isle, new Vector3(i, 0, j), isle.transform.rotation);
                instance.tag = "Procedural";
                if (randomize) {
                    isle = isleAssets[Random.Range(0, isleAssets.Count)]; //not a good idea with different sizes...
                    isleSize = isle.GetComponent<BoxCollider>().size;
                }
            }
        }
    }
    private void LayoutStartExitPlayer() {
        //Start on bottom left (0 x levelDefinitionMapHeight)
        GameObject start = Instantiate(startAsset, new Vector3(mapHeight - 5.5f, 0, 7),
            startAsset.transform.rotation);
        start.tag = "Procedural";

        //Exit on bottom right (levelDefinitionMapWith x levelDefinitionMapHeight)
        GameObject exit = Instantiate(exitAsset, new Vector3(mapHeight - 6.5f, 0, mapWith - 13),
            exitAsset.transform.rotation);
        exit.tag = "Procedural";
    }

    private void SpawnNPCs(int quantity, List<GameObject> prefabsList, string npcTag) {
        for (int i = 0; i < quantity; i++) {
            GameObject npcPrefab = prefabsList[Random.Range(0, prefabsList.Count)];
            Vector3 position = GetNPCRandomPosition();
            GameObject instance = Instantiate(npcPrefab, position, npcPrefab.transform.rotation);
            instance.tag = npcTag;
            NPCController npcController = instance.GetComponent<NPCController>();
            npcController.SetMapDimensions(mapHeight, mapWith);

            // if spawn inside something try to move it out
            int maxTries = 10;
            Collider[] colliders = new Collider[5];
            while (Physics.OverlapSphereNonAlloc(instance.transform.position, 1.0f, colliders) > 1 && maxTries > 0) {
                instance.transform.position = GetNPCRandomPosition();
                maxTries--;
            }
        }
    }
    private void SpawnPowerups(int quantity, List<GameObject> prefabsList, string powerupTag) {
        for (int i = 0; i < quantity; i++) {
            GameObject npcPrefab = prefabsList[Random.Range(0, prefabsList.Count)];
            Vector3 position = GetNPCRandomPosition();
            GameObject instance = Instantiate(npcPrefab, position, npcPrefab.transform.rotation);
            instance.tag = powerupTag;

            // if spawn inside something try to move it out
            int maxTries = 10;
            Collider[] colliders = new Collider[5];
            while (Physics.OverlapSphereNonAlloc(instance.transform.position, 1.0f, colliders) > 1 && maxTries > 0) {
                instance.transform.position = GetNPCRandomPosition();
                maxTries--;
            }
        }
    }
    private void LayoutPlayer() {
        GameObject player = GameObject.FindWithTag("Player");
        player.transform.position =
            new Vector3(mapHeight - 5.5f, 1, 4.5f); //TODO FIXME: doest update position on run...
    }
    private Vector3 GetNPCRandomPosition() {
        return new Vector3(Random.Range(isleVertMargin, mapHeight - isleVertMargin), 1,
            Random.Range(isleHorMargin, mapWith - isleHorMargin));
    }

    private void LayoutRoad() {
        //Layout bare road
        GameObject bareRoad = roads[0];
        Vector3 bareRoadSize = new Vector3(5, 0, 5);
        for (float i = -EXTERIOR_MARGIN; i <= mapHeight + EXTERIOR_MARGIN; i += bareRoadSize.x) {
            for (float j = -EXTERIOR_MARGIN; j < mapWith + EXTERIOR_MARGIN; j += bareRoadSize.z) {
                GameObject instance = Instantiate(bareRoad, new Vector3(i, -0.05f, j), Quaternion.identity);
                instance.tag = "Procedural";
            }
        }

        //Layout Parking lines
        GameObject parkingLine = roads[1];
        Vector3 parkingLineSize = new Vector3(5, 0, 10);
        for (float i = mapHeight; i <= mapHeight + EXTERIOR_MARGIN; i += parkingLineSize.x * 3) {
            for (float j = 10; j < mapWith; j += parkingLineSize.z) {
                GameObject instance = Instantiate(parkingLine, new Vector3(i, 0, j), Quaternion.identity);
                instance.tag = "Procedural";
            }
        }
    }

    private void LayoutSidewalk() {
        GameObject sidewalk = sidewalks[0];
        Vector3 sidewalkSize = new Vector3(5, 0, 5);
        //top sidewalk
        for (float j = 0; j < mapWith; j += sidewalkSize.x) {
            GameObject instance = Instantiate(sidewalk, new Vector3(-10, 0, j + 5),
                Quaternion.AngleAxis(90, Vector3.up));
            instance.tag = "Procedural";
        }

        //left sidewalk
        for (float i = 0; i < mapHeight; i += sidewalkSize.z) {
            GameObject instance = Instantiate(sidewalk, new Vector3(i - 5, 0, -5), Quaternion.identity);
            instance.tag = "Procedural";
        }

        //right sidewalk
        for (float i = 0; i < mapHeight; i += sidewalkSize.z) {
            GameObject instance = Instantiate(sidewalk, new Vector3(i, 0, mapWith + 5),
                Quaternion.AngleAxis(180, Vector3.up));
            instance.tag = "Procedural";
        }

        //bottom sidewalk
        for (float j = 0; j < mapWith; j += sidewalkSize.x) {
            GameObject instance = Instantiate(sidewalk, new Vector3(mapHeight, 0, j),
                Quaternion.AngleAxis(-90, Vector3.up));
            instance.tag = "Procedural";
        }

        GameObject sidewalkCorner = sidewalks[1];
        GameObject cornerInstance;
        //top left corner
        cornerInstance = Instantiate(sidewalkCorner, new Vector3(-10, 0, -5), Quaternion.identity);
        cornerInstance.tag = "Procedural";
        //top right corner
        cornerInstance = Instantiate(sidewalkCorner, new Vector3(-10, 0, mapWith + 5),
            Quaternion.AngleAxis(90, Vector3.up));
        cornerInstance.tag = "Procedural";
        //bottom left corner
        cornerInstance = Instantiate(sidewalkCorner, new Vector3(mapHeight, 0, -5),
            Quaternion.AngleAxis(-90, Vector3.up));
        cornerInstance.tag = "Procedural";
        //bottom right corner
        cornerInstance = Instantiate(sidewalkCorner, new Vector3(mapHeight, 0, mapWith + 5),
            Quaternion.AngleAxis(180, Vector3.up));
        cornerInstance.tag = "Procedural";

    }
}