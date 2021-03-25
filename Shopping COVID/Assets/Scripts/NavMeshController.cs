using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;

public class NavMeshController : MonoBehaviour {
    private NavMeshSurface[] surfaces;

    private void Start() {
        surfaces = FindObjectsOfType<NavMeshSurface>();
    }
    public void Bake() {
        foreach (NavMeshSurface surface in surfaces) {
            surface.BuildNavMesh();
        }
    }

    public void Clear() {
        foreach (NavMeshSurface surface in surfaces) {
            surface.RemoveData();
        }
        SceneView.RepaintAll();
    }
    
}
