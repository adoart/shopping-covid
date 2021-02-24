using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isGameActive;
    [SerializeField] private GameObject titleScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject gameoverScreen;
    [SerializeField] private LifeBarController lifeBarController;

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public bool IsGameActive {
        get => isGameActive;
    }

    public void DecreaseLife() {
        lifeBarController.DecreaseLife();

    }

    public void GameOver() {
        Debug.Log("You got COVID!!!");
        isGameActive = false;
        gameoverScreen.SetActive(true);
    }

    internal void Win() {
        Debug.Log("Win!!!"); //TODO add next level button...
        isGameActive = false;
        winScreen.SetActive(true);
    }

    internal void ForgotItemWarning() {
        Debug.Log("Forgot the Milk!!!");
        throw new NotImplementedException();
    }
}
