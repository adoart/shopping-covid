using System.Collections.Generic;
using DefaultNamespace;
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

    private int currentLevelDefinitionIndex;
    [SerializeField]
    private List<LevelDefinition> levelDefinitions;
    [FormerlySerializedAs("levelDefinition")]
    [SerializeField]
    private LevelDefinition currentLevelDefinition;
    [SerializeField]
    private GameManager gameManager;

    [Space]
    [Header("Interior")]
    [SerializeField]
    private List<GameObject> floorAssets;
    [SerializeField]
    private List<GameObject> wallAssets;
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
    [SerializeField]
    private List<GameObject> wetFloors;
    [SerializeField]
    private List<GameObject> items;

    [SerializeField]
    private List<ItemPickUps_SO> items_SO;

    private const int EXTERIOR_MARGIN = 50;
    [Space]
    [Header("Exterior")]
    [SerializeField]
    private List<GameObject> sidewalks;
    [SerializeField]
    private List<GameObject> roads;
    [SerializeField]
    private List<GameObject> seaSides;
    [SerializeField]
    private List<GameObject> buildings;
    [SerializeField]
    private List<GameObject> cars;


    [SerializeField]
    private NavMeshSurface[] surfaces;
    private GameObject exitInstance;
    [SerializeField]
    private bool randomize;

    private int mapHeight;
    private int mapWith;

    // Start is called before the first frame update
    void Start() {
        currentLevelDefinition = levelDefinitions[LevelDefinitionHelper.GetCurrentLevelIndex()];
        UpdateMap();
    }

    public void SetNextLevel() {
        currentLevelDefinition = levelDefinitions[LevelDefinitionHelper.SetNextLevelIndex()];
    }

    public bool HasNextLevel() {
        return LevelDefinitionHelper.GetCurrentLevelIndex() < levelDefinitions.Count - 1;
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
        GameObject[] shelfAssets = GameObject.FindGameObjectsWithTag("Shelf");
        foreach (GameObject shelfAsset in shelfAssets) {
            DestroyImmediate(shelfAsset);
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
        GameObject[] itemAssets = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject trolleyAsset in itemAssets) {
            DestroyImmediate(trolleyAsset);
        }
        GameObject[] exitAssets = GameObject.FindGameObjectsWithTag("Exit");
        foreach (GameObject exitAsset in exitAssets) {
            DestroyImmediate(exitAsset);
        }
    }

    public void BuildMap() {
        mapHeight = currentLevelDefinition.mapHeight;
        mapWith = currentLevelDefinition.mapWith;
        //Layout floor
        LayoutFloor();
        //Layout walls stalls
        LayoutWalls();
        //Layout isle stalls
        LayoutIsles();
        //Layout Start & Exit
        LayoutStartExitPlayer();
        //Spawn NPCs
        SpawnNPCs(currentLevelDefinition.numberOfNPCs, npcs, "NPC");
        SpawnNPCs(currentLevelDefinition.numberOfEnemies, enemies, "Enemy");
        //Spawn Powerups
        SpawnPowerups(currentLevelDefinition.numberOfMaskPowerUps, masks, "Mask");
        SpawnPowerups(currentLevelDefinition.numberOfTrolleyPowerUps, trolleys, "Trolley");
        SpawnPowerups(currentLevelDefinition.numberOfWetFloors, wetFloors, "Procedural");
        SpawnItems(currentLevelDefinition.numberOfItems, items, "Item");
        LayoutPlayer();

        //Layout Exterior
        //Layout Sidewalks
        LayoutSidewalk();
        //Layout Road
        LayoutRoad();
        //Layout top buildings
        LayoutTopBuildings();
        //Layout right buildings
        LayoutRightBuildings();
    }
    private void LayoutFloor() {
        GameObject floor = floorAssets[Random.Range(0, floorAssets.Count)];
        Vector3 floorSize = new Vector3(5, 0, 5);
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
            instance.tag = "Shelf";
        }

        //left wall
        for (float i = 0; i < (mapHeight - wallSize.x); i += wallSize.x) {
            GameObject instance = Instantiate(wall, new Vector3(i + 5, 0, 1),
                wall.transform.rotation);
            instance.tag = "Shelf";
        }

        //right wall
        for (float i = 0; i < (mapHeight - wallSize.x); i += wallSize.x) {
            GameObject instance = Instantiate(wall, new Vector3(i - 2.75f, 0, (mapWith) - 1),
                Quaternion.AngleAxis(180, Vector3.up));
            instance.tag = "Shelf";
        }

        //bottom wall
        Vector3 invisibleWallSize = invisibleWall.GetComponent<BoxCollider>().size;
        for (float j = 0;
            j < (mapWith - invisibleWallSize.x * 2);
            j += invisibleWallSize.x) {
            //leave one blank space to place the "Exit" wall
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
        Vector3 isleSize = Vector3.zero; //isle.GetComponent<BoxCollider>().size;
        float maxIsleHeight = 0;
        for (float i = isleVertMargin;
            i < mapHeight - (maxIsleHeight + isleVertMargin);
            i += maxIsleHeight + isleVertMargin) {
            maxIsleHeight = 0;
            for (float j = isleHorMargin; j < mapWith - (isleSize.z + isleHorMargin); j += isleSize.z + isleHorMargin) {
                GameObject isle = randomize
                    ? currentLevelDefinition.isleAssets[Random.Range(0, currentLevelDefinition.isleAssets.Count)]
                    : currentLevelDefinition.isleAssets[1];
                isleSize = isle.GetComponent<BoxCollider>().size;
                if (isleSize.x > maxIsleHeight) {
                    maxIsleHeight = isleSize.x;
                }
                GameObject instance = Instantiate(isle, new Vector3(i, 0, j), isle.transform.rotation);
                instance.tag = "Shelf";
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
        List<GameObject> spawnNPCs = Spawn(quantity, prefabsList, npcTag);
        foreach (GameObject npc in spawnNPCs) {
            NPCController npcController = npc.GetComponent<NPCController>();
            npcController.SetMapDimensions(mapHeight, mapWith);
        }
    }
    private void SpawnPowerups(int quantity, List<GameObject> prefabsList, string powerupTag) {
        Spawn(quantity, prefabsList, powerupTag);
    }

    private void SpawnItems(int quantity, List<GameObject> prefabsList, string itemTag) {
        // gameManager.SetSpawnedItems(Spawn(quantity, prefabsList, itemTag));
        gameManager.SetSpawnedItems(SpawnItems_SO(quantity, items_SO));
    }

    private List<GameObject> SpawnItems_SO(int quantity, List<ItemPickUps_SO> prefabsList) {
        List<GameObject> instances = new List<GameObject>(quantity);

        for (int i = 0; i < quantity; i++) {
            ItemPickUps_SO prefab = prefabsList[Random.Range(0, prefabsList.Count)];
            GameObject emptyGameObject = new GameObject(prefab.itemName);
            Vector3 position = GetRandomPosition();
            GameObject instance = Instantiate(emptyGameObject, position, prefab.itemSpawnObject.transform.rotation);
            GameObject item = Instantiate(prefab.itemSpawnObject, instance.transform);
            item.AddComponent<ItemAnimation>();

            instance.tag = prefab.itemTag;
            item.tag = prefab.itemTag;

            // if spawn inside something try to move it out
            int maxTries = 10;
            Collider[] colliders = new Collider[5];
            while (Physics.OverlapSphereNonAlloc(instance.transform.position, 1.0f, colliders) > 1 && maxTries > 0) {
                instance.transform.position = GetRandomPosition();
                maxTries--;
            }

            //add camera at final position
            if (prefab.itemCameraObject != null) {
                GameObject itemCamera = Instantiate(prefab.itemCameraObject, instance.transform);
                itemCamera.SetActive(false);
            }
            instances.Add(instance);

            DestroyImmediate(emptyGameObject);
        }

        instances[0].transform.GetChild(1).gameObject.SetActive(true);
        return instances;
    }
    private List<GameObject> Spawn(int quantity, List<GameObject> prefabsList, string tag) {
        List<GameObject> instances = new List<GameObject>(quantity);
        for (int i = 0; i < quantity; i++) {
            GameObject prefab = prefabsList[Random.Range(0, prefabsList.Count)];
            Vector3 position = GetRandomPosition();
            GameObject instance = Instantiate(prefab, position, prefab.transform.rotation);
            instance.tag = tag;
            instances.Add(instance);

            // if spawn inside something try to move it out
            int maxTries = 10;
            Collider[] colliders = new Collider[5];
            while (Physics.OverlapSphereNonAlloc(instance.transform.position, 1.0f, colliders) > 1 && maxTries > 0) {
                instance.transform.position = GetRandomPosition();
                maxTries--;
            }
        }

        return instances;
    }
    private void LayoutPlayer() {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.Warp(new Vector3(mapHeight - 5.5f, 1, 4.5f));
    }
    private Vector3 GetRandomPosition() {
        return new Vector3(Random.Range(isleVertMargin, mapHeight - isleVertMargin), 1,
            Random.Range(isleHorMargin, mapWith - isleHorMargin));
    }

    private void LayoutRoad() {
        //Layout bare road
        GameObject bareRoad = roads[0];
        Vector3 bareRoadSize = new Vector3(5, 0, 5);
        for (float i = -EXTERIOR_MARGIN; i <= mapHeight + EXTERIOR_MARGIN; i += bareRoadSize.x) {
            for (float j = -EXTERIOR_MARGIN + 30; j < mapWith + EXTERIOR_MARGIN; j += bareRoadSize.z) {
                GameObject instance = Instantiate(bareRoad, new Vector3(i, -0.05f, j), Quaternion.identity);
                instance.tag = "Procedural";
            }
        }

        //Layout Parking lines
        GameObject parkingLine = roads[1];
        Vector3 parkingLineSize = new Vector3(5, 0, 10);
        for (float i = mapHeight; i <= mapHeight + EXTERIOR_MARGIN; i += parkingLineSize.x * 3) {
            for (float j = 10; j < mapWith; j += parkingLineSize.z) {
                GameObject instance = Instantiate(parkingLine, new Vector3(i, -0.04f, j), Quaternion.identity);
                instance.tag = "Procedural";
                //Spawn random cars
                if (Random.Range(0, 3) == 1) {
                    GameObject carInstance = Instantiate(cars[Random.Range(0, cars.Count)], new Vector3(i, 0, j + 1),
                        Quaternion.identity);
                    carInstance.tag = "Procedural";
                }
            }
        }

        //Layout Crossing lane
        GameObject crossingLane = roads[2];
        Vector3 crossingLaneSize = new Vector3(5, 0, 5);
        for (float i = mapHeight; i <= mapHeight + EXTERIOR_MARGIN; i += crossingLaneSize.x) {
            GameObject instance = Instantiate(crossingLane, new Vector3(i, -0.04f, 2),
                Quaternion.AngleAxis(90, Vector3.up));
            instance.tag = "Procedural";
        }

        //Layout Road arrows
        GameObject roadArrow = roads[3];
        Vector3 roadArrowSize = new Vector3(5, 0, 5);
        for (float i = mapHeight + 7; i <= mapHeight + EXTERIOR_MARGIN; i += roadArrowSize.x * 3) {
            GameObject instance = Instantiate(roadArrow, new Vector3(i, -0.04f, 10), Quaternion.identity);
            instance.tag = "Procedural";
        }

        //Layout main road (left)
        GameObject roadLine = roads[4];
        GameObject roadDivider = roads[5];
        Vector3 roadLineSize = new Vector3(5, 0, 5);
        for (float i = -EXTERIOR_MARGIN; i <= mapHeight + EXTERIOR_MARGIN; i += roadLineSize.x) {
            GameObject instance = Instantiate(roadLine, new Vector3(i, -0.04f, -20), Quaternion.identity);
            instance.tag = "Procedural";
            instance = Instantiate(roadLine, new Vector3(i, -0.04f, -15), Quaternion.identity);
            instance.tag = "Procedural";
            instance = Instantiate(roadDivider, new Vector3(i, -0.04f, -15), Quaternion.identity);
            instance.tag = "Procedural";
        }

        //Layout Sea side
        GameObject oceanTile = seaSides[0];
        Vector3 oceanTileSize = new Vector3(30, 0, 30);
        for (float i = -EXTERIOR_MARGIN; i <= mapHeight + EXTERIOR_MARGIN; i += oceanTileSize.x) {
            GameObject instance = Instantiate(oceanTile, new Vector3(i, -5, -50), Quaternion.identity);
            instance.tag = "Procedural";
        }

        GameObject waterEdge = seaSides[1];
        Vector3 waterEdgeSize = new Vector3(5, 5, 1);
        for (float i = -EXTERIOR_MARGIN; i <= mapHeight + EXTERIOR_MARGIN; i += waterEdgeSize.x) {
            GameObject instance = Instantiate(waterEdge, new Vector3(i, 0, -21), Quaternion.identity);
            instance.tag = "Procedural";
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
    private void LayoutTopBuildings() {

        //Layout sidewalk
        GameObject sidewalk = sidewalks[0];
        GameObject sidewalkCorner = sidewalks[1];
        GameObject cornerInstance;
        Vector3 sidewalkSize = new Vector3(5, 0, 5);
        //top sidewalk
        for (float j = 0; j < mapWith + EXTERIOR_MARGIN; j += sidewalkSize.x) {
            GameObject instance = Instantiate(sidewalk, new Vector3(-20, 0, j), Quaternion.AngleAxis(-90, Vector3.up));
            instance.tag = "Procedural";
        }
        //left sidewalk
        for (float i = -30; i >= -EXTERIOR_MARGIN; i -= sidewalkSize.z) {
            GameObject instance = Instantiate(sidewalk, new Vector3(i, 0, -5), Quaternion.identity);
            instance.tag = "Procedural";
        }
        //corner sidewalk
        cornerInstance = Instantiate(sidewalkCorner, new Vector3(-20, 0, -5), Quaternion.AngleAxis(-90, Vector3.up));
        cornerInstance.tag = "Procedural";

        //Layout buildings
        GameObject residentialBuilding = buildings[0];
        Vector3 residentialBuildingSize = new Vector3(5, 12, 5);
        //top residential buildings
        for (float j = 4; j < mapWith + EXTERIOR_MARGIN; j += residentialBuildingSize.x) {
            GameObject instance = Instantiate(residentialBuilding, new Vector3(-30, 0, j), Quaternion.identity);
            instance.tag = "Procedural";
        }
        //left residential buildings
        for (float i = -36; i >= -EXTERIOR_MARGIN; i -= residentialBuildingSize.z) {
            GameObject instance = Instantiate(residentialBuilding, new Vector3(i, 0, 5),
                Quaternion.AngleAxis(90, Vector3.up));
            instance.tag = "Procedural";
        }
        //Corner residential building
        GameObject cornerResidentialBuilding = buildings[1];
        GameObject cornerResidentialBuildingInstance =
            Instantiate(cornerResidentialBuilding, new Vector3(-30, 0, 0), Quaternion.identity);
        cornerResidentialBuildingInstance.tag = "Procedural";
    }
    private void LayoutRightBuildings() {
        GameObject sidewalk = sidewalks[0];
        Vector3 sidewalkSize = new Vector3(5, 0, 5);
        //top sidewalk
        for (float j = mapWith + 20; j < mapWith + EXTERIOR_MARGIN; j += sidewalkSize.x) {
            GameObject instance = Instantiate(sidewalk, new Vector3(-10, 0, j + 5),
                Quaternion.AngleAxis(90, Vector3.up));
            instance.tag = "Procedural";
        }

        //left sidewalk
        for (float i = -5; i < mapHeight - 5; i += sidewalkSize.z) {
            GameObject instance = Instantiate(sidewalk, new Vector3(i, 0, mapWith + 15), Quaternion.identity);
            instance.tag = "Procedural";
        }

        //bottom sidewalk
        for (float j = mapWith + 20; j < mapWith + EXTERIOR_MARGIN; j += sidewalkSize.x) {
            GameObject instance = Instantiate(sidewalk, new Vector3(mapHeight, 0, j),
                Quaternion.AngleAxis(-90, Vector3.up));
            instance.tag = "Procedural";
        }

        //Corners sidewalk
        GameObject sidewalkCorner = sidewalks[1];
        GameObject cornerInstance;
        //top left corner
        cornerInstance = Instantiate(sidewalkCorner, new Vector3(-10, 0, mapWith + 15), Quaternion.identity);
        cornerInstance.tag = "Procedural";
        //bottom left corner
        cornerInstance = Instantiate(sidewalkCorner, new Vector3(mapHeight, 0, mapWith + 15),
            Quaternion.AngleAxis(-90, Vector3.up));
        cornerInstance.tag = "Procedural";

        //Layout buildings
        GameObject officeBuilding = buildings[2];
        Vector3 officeBuildingSize = new Vector3(10, 12, 10);
        //top office buildings
        for (float j = mapWith + 40; j <= mapWith + EXTERIOR_MARGIN; j += officeBuildingSize.x) {
            GameObject officeInstance =
                Instantiate(officeBuilding, new Vector3(-5, 0, j), Quaternion.AngleAxis(90, Vector3.up));
            officeInstance.tag = "Procedural";
        }
        //left office buildings
        for (float i = 5; i < mapHeight - 15; i += officeBuildingSize.z) {
            GameObject instance = Instantiate(officeBuilding, new Vector3(i, 0, mapWith + 20), Quaternion.identity);
            instance.tag = "Procedural";
        }
        //bottom office buildings
        for (float j = mapWith + 30; j < mapWith + EXTERIOR_MARGIN; j += officeBuildingSize.x) {
            GameObject officeInstance = Instantiate(officeBuilding, new Vector3(mapHeight - 5, 0, j),
                Quaternion.AngleAxis(-90, Vector3.up));
            officeInstance.tag = "Procedural";
        }
        //Corner office building
        GameObject cornerOfficeBuilding = buildings[3];
        GameObject cornerOfficeBuildingInstance =
            Instantiate(cornerOfficeBuilding, new Vector3(-5, 0, mapWith + 22), Quaternion.identity);
        cornerOfficeBuildingInstance.tag = "Procedural";
        cornerOfficeBuildingInstance = Instantiate(cornerOfficeBuilding, new Vector3(mapHeight - 15, 0, mapWith + 22),
            Quaternion.identity);
        cornerOfficeBuildingInstance.tag = "Procedural";
    }
}