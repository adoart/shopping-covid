using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour {
    private bool isGameActive;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private GameObject forgotTextBubble;
    [SerializeField] private MoveItemPopup popupPanel;
    [SerializeField] private LifeBarController lifeBarController;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private SceneBuilder sceneBuilder;

    private AudioSource audioSource;
    [SerializeField] private AudioClip gameoverSound;
    [SerializeField] private AudioClip winSound;

    [SerializeField] private int lifes = 3;
    private PlayerController player;
    [FormerlySerializedAs("hasItem")]
    public bool hasAllItems = false;

    private List<GameObject> spawnedItems;

    private void Awake() {
#if UNITY_EDITOR //Disable all logging on release builds.
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = true;
#endif
    }

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerController>();
        player.HitByEnemy += DecreaseLife;
        player.PickedUpItem += PickupItem;
        player.TriedToExit += TryToExit;
        spawnedItems = new List<GameObject>();
        StartGame();
    }
    private void TryToExit() {
        if (hasAllItems) {
            Win();
        } else {
            StartCoroutine(ForgotCountdownCoroutine());
        }
    }

    IEnumerator ForgotCountdownCoroutine() {
        forgotTextBubble.SetActive(true);
        yield return new WaitForSeconds(7);
        forgotTextBubble.SetActive(false);
    }

    private void PickupItem(GameObject item) {
        if (spawnedItems.Count > 0) {
            spawnedItems.Remove(item);
            if (spawnedItems.Count > 0) {
                spawnedItems[0].transform.GetChild(1).gameObject.SetActive(true);
                popupPanel.itemTransform = spawnedItems[0].transform;
            }
        }
        hasAllItems = spawnedItems.Count == 0;
    }

    public void StartGame() {
        isGameActive = true;
        titleScreen.SetActive(false);
    }

    public void ResetGame() {
        //Load Title screen
        SceneManager.LoadScene(0);
    }

    public void NextLevel() {
        //Load next level
        levelLoader.LoadNextLevel();
    }

    public void NextProceduralLevel() {
        StartCoroutine(LoadProceduralLevel());
    }

    IEnumerator LoadProceduralLevel() {
        //Load next LevelDefinition and generate procedural level
        levelLoader.FadeOut();
        sceneBuilder.SetNextLevel();

        //Wait
        yield return new WaitForSeconds(levelLoader.transitionTime);

        //Load scene
        sceneBuilder.UpdateMap();
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame() {
        ResetGame();
    }

    public bool IsGameActive
    {
        get => isGameActive;
    }

    private void DecreaseLife() {
        lifeBarController.DecreaseLife();
        if (isGameActive) {
            if (lifes > 0) {
                lifes--;
            }

            if (lifes <= 0) {
                GameOver();
            }
        }
    }

    private void GameOver() {
        Debug.Log("You got COVID!!!");
        audioSource.PlayOneShot(gameoverSound);
        isGameActive = false;
        gameoverScreen.SetActive(true);
    }

    private void Win() {
        Debug.Log("Win!!!"); //TODO add next level button...
        audioSource.PlayOneShot(winSound);
        isGameActive = false;
        winScreen.SetActive(true);
        if (sceneBuilder != null && !sceneBuilder.HasNextLevel()) {
            nextLevelButton.SetActive(false);
        }
    }
    public void SetSpawnedItems(List<GameObject> spawnedItems) {
        this.spawnedItems = spawnedItems;
        this.spawnedItems[0].SetActive(true);
        popupPanel.itemTransform = this.spawnedItems[0].transform;
    }
}