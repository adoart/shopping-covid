using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonScript : MonoBehaviour
{
    private void Awake() {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Singleton");
        if (gameObjects.Length > 1) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
