using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.AI;

public class SceneBuilder : MonoBehaviour {
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
    private NavMeshSurface[] surfaces;
    private GameObject exitInstance;
    // Start is called before the first frame update
    void Start() {
        surfaces = FindObjectsOfType<NavMeshSurface>();
        BuildMap(levelDefinition.mapHeight, levelDefinition.mapWith);
        foreach (NavMeshSurface surface in surfaces) {
            surface.BuildNavMesh();
        }
    }

    public void UpdateMap() {
        ClearMap();
        BuildMap(levelDefinition.mapHeight, levelDefinition.mapWith);
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
        if (exitInstance != null) {
            DestroyImmediate(exitInstance);
        }
    }

    public void BuildMap(int levelDefinitionMapHeight, int levelDefinitionMapWith) {
        //Layout floor
        LayoutFloor(levelDefinitionMapHeight, levelDefinitionMapWith);
        //Layout walls stalls
        LayoutWalls(levelDefinitionMapHeight, levelDefinitionMapWith);
        //Layout isle stalls
        LayoutIsles(levelDefinitionMapHeight, levelDefinitionMapWith);
        //Layout Start & Exit
        LayoutStartExit(levelDefinitionMapHeight, levelDefinitionMapWith);
    }
    private void LayoutFloor(int levelDefinitionMapHeight, int levelDefinitionMapWith) {
        GameObject floor = floorAssets[Random.Range(0, floorAssets.Count)];
        Vector3 floorSize = floor.GetComponent<BoxCollider>().size;
        for (float i = 0; i <= (levelDefinitionMapHeight - floorSize.x); i += floorSize.x) {
            for (float j = 0; j < levelDefinitionMapWith; j += floorSize.z) {
                GameObject instance = Instantiate(floor, new Vector3(i, 0, j), floor.transform.rotation);
                instance.tag = "Procedural";
            }
        }
    }
    private void LayoutWalls(int levelDefinitionMapHeight, int levelDefinitionMapWith) {
        GameObject wall = wallAssets[0];
        Vector3 wallSize = wall.GetComponent<BoxCollider>().size;
        //top wall
        for (float j = 0; j < (levelDefinitionMapWith - wallSize.x); j += wallSize.x) {
            GameObject instance = Instantiate(wall, new Vector3(-2.75f, 0, j + 1),
                Quaternion.AngleAxis(90, Vector3.up));
            instance.tag = "Procedural";
        }

        //left wall
        for (float i = 0; i < (levelDefinitionMapHeight - wallSize.x); i += wallSize.x) {
            GameObject instance = Instantiate(wall, new Vector3(i + 5, 0, 1),
                wall.transform.rotation);
            instance.tag = "Procedural";
        }

        //right wall
        for (float i = 0; i < (levelDefinitionMapHeight - wallSize.x); i += wallSize.x) {
            GameObject instance = Instantiate(wall, new Vector3(i - 2.75f, 0, (levelDefinitionMapWith) - 1),
                Quaternion.AngleAxis(180, Vector3.up));
            instance.tag = "Procedural";
        }

        //bottom wall
        Vector3 invisibleWallSize = invisibleWall.GetComponent<BoxCollider>().size;
        for (float j = 0;
            j < (levelDefinitionMapWith - invisibleWallSize.x * 2);
            j += invisibleWallSize.x) { //leave one blank space to place the "Exit" wall
            GameObject instance =
                Instantiate(invisibleWall, new Vector3(levelDefinitionMapHeight - 5, 2, j + 4),
                    invisibleWall.transform.rotation);
            instance.tag = "Procedural";
        }

        //Exit wall
        exitInstance = Instantiate(exitInvisibleWall,
            new Vector3(levelDefinitionMapHeight - 5.5f, 0, levelDefinitionMapWith - 11),
            exitInvisibleWall.transform.rotation);
        exitInstance.tag = "Exit";
    }
    private void LayoutIsles(int levelDefinitionMapHeight, int levelDefinitionMapWith) {
        GameObject isle = isleAssets[0 /*Random.Range(0, isleAssets.Count)*/];
        Vector3 isleSize = isle.GetComponent<BoxCollider>().size;
        for (float i = 4; i < levelDefinitionMapHeight - isleSize.x; i += isleSize.x * 1.5f) {
            for (float j = 5; j < levelDefinitionMapWith - isleSize.z; j += isleSize.z * 3f) {
                GameObject instance = Instantiate(isle, new Vector3(i, 0, j), isle.transform.rotation);
                instance.tag = "Procedural";
                // isle = isleAssets[Random.Range(0, isleAssets.Count)]; //not a good idea with different sizes...
                // isleSize = isle.GetComponent<BoxCollider>().size;
            }
        }
    }
    private void LayoutStartExit(int levelDefinitionMapHeight, int levelDefinitionMapWith) {
        //Start on bottom left (0 x levelDefinitionMapHeight)
        GameObject start = Instantiate(startAsset, new Vector3(levelDefinitionMapHeight - 5.5f, 0, 7),
            startAsset.transform.rotation);
        start.tag = "Procedural";

        //Exit on bottom right (levelDefinitionMapWith x levelDefinitionMapHeight)
        GameObject exit = Instantiate(exitAsset,
            new Vector3(levelDefinitionMapHeight - 6.5f, 0, levelDefinitionMapWith - 13),
            exitAsset.transform.rotation);
        exit.tag = "Procedural";

    }
}