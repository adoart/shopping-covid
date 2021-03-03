using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    private Animator playerAnimator;
    private GameManager gameManager;
    private AudioSource audioSource;
    [SerializeField] private GameObject powerupIndicator;
    [SerializeField] private AudioClip powerupPickupSound;
    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioClip gameoverSound;
    [SerializeField] private GameObject forgotTextBubble;
    [SerializeField] private Vector3 forgotBubbleOffset;


    public int startingLife = 3;
    private int life;
    public bool hasItem = false;
    public bool hasMask = false;
    public bool hasTrolley = false;
    

    void Start() {
        life = startingLife;
        audioSource = GetComponent<AudioSource>();
        playerAnimator = GetComponentInChildren<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        powerupIndicator.SetActive(false);
    }
    // Update is called once per frame
    void Update() {
        MouseMovePlayer();
        UpdateForgotBubblePosition();

        //Update animator speed to control animation.
        playerAnimator.SetFloat("Speed_f", agent.velocity.magnitude);
    }

    private void UpdateForgotBubblePosition() {
        if (forgotTextBubble) {
            forgotTextBubble.transform.position = Camera.main.WorldToScreenPoint(transform.position + forgotBubbleOffset);
        }
    }

    public void LooseLife() {
        if (gameManager != null && gameManager.IsGameActive) {
            if (!hasMask && life > 0) {
                gameManager.DecreaseLife();
                life--;
            }

            if (life <= 0) {
                ResetLife();
                audioSource.PlayOneShot(gameoverSound);
                gameManager.GameOver();
            }
        }
    }

    private void ResetLife() {
        life = startingLife;
    }

    private void MouseMovePlayer() {
        if (Input.GetMouseButtonDown(0) && gameManager != null && gameManager.IsGameActive) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) {
                agent.SetDestination(hit.point);
            }
        }
    }

    IEnumerator MaskPowerupCountdownCoroutine() {
        powerupIndicator.SetActive(true);
        hasMask = true;
        yield return new WaitForSeconds(7);
        powerupIndicator.SetActive(false);
        hasMask = false;
    }

    IEnumerator ForgotCountdownCoroutine() {
        forgotTextBubble.SetActive(true);
        yield return new WaitForSeconds(7);
        forgotTextBubble.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision) {
        //Grab the Item
        if (collision.gameObject.CompareTag("Item")) {
            hasItem = true;
            audioSource.PlayOneShot(itemPickupSound);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Mask")) {
            audioSource.PlayOneShot(powerupPickupSound);
            Destroy(collision.gameObject);
            StartCoroutine(MaskPowerupCountdownCoroutine());
        }

        if (collision.gameObject.CompareTag("Trolley")) {
            audioSource.PlayOneShot(powerupPickupSound);
            hasTrolley = true;
            collision.transform.parent = transform;
        }

        //Exit Level
        if (collision.gameObject.CompareTag("Exit")) {
            if (hasItem) {
                audioSource.PlayOneShot(winSound);
                gameManager.Win();
            } else {
                StartCoroutine(ForgotCountdownCoroutine());
            }
        }

    }
}
