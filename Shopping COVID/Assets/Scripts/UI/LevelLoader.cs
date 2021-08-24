using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    public Animator transition;
    public float transitionTime = 1.0f;
    public void LoadNextLevel() {
        //Load Next Level
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void FadeOut() {
        //Reload same Scene, reset state (for procedural levels)
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    public void FadeIn() {
        //Reload same Scene (for procedural levels)
        //Play animation
        transition.SetTrigger("Start");
    }

    IEnumerator LoadLevel(int levelIndex) {
        //Play animation
        transition.SetTrigger("Start");

        //Wait
        yield return new WaitForSeconds(transitionTime);

        //Load scene
        SceneManager.LoadScene(levelIndex);
    }
}