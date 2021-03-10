using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    private bool isGameActive;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private GameObject forgotTextBubble;
    [SerializeField] private LifeBarController lifeBarController;

    private AudioSource audioSource;
    [SerializeField] private AudioClip gameoverSound;
    [SerializeField] private AudioClip winSound;

    [SerializeField] private int lifes = 3;
    private PlayerController player;
    public bool hasItem = false;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerController>();
        player.HitByEnemy += DecreaseLife;
        player.PickedUpItem += PickupItem;
        player.TriedToExit += TryToExit;
        StartGame();
    }
    private void TryToExit() {
        if (hasItem) {
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
    
    private void PickupItem() {
        hasItem = true;
    }

    public void StartGame() {
        isGameActive = true;
        titleScreen.SetActive(false);
    }

    public void ResetGame() {
        //Load Title screen
        SceneManager.LoadScene(0);
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
    }
}
