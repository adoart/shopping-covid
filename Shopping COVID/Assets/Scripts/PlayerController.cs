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

    public int startingLife = 3;
    private int life;
    public bool hasItem = false;
    public bool hasMask = false;
    public bool hasTrolley = false;
    

    void Start() {
        life = startingLife;
        playerAnimator = GetComponentInChildren<Animator>();
        gameManager = FindObjectOfType<GameManager>();
    }
    // Update is called once per frame
    void Update() {
        MouseMovePlayer();

        //Update animator speed to control animation.
        playerAnimator.SetFloat("Speed_f", agent.velocity.magnitude);
    }

    public void LooseLife() {
        if (gameManager != null && gameManager.IsGameActive) {
            if (!hasMask && life > 0) {
                life--;
            }

            if (life <= 0) {
                ResetLife();
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
        yield return new WaitForSeconds(7);
        hasMask = false;
    }

    private void OnCollisionEnter(Collision collision) {
        //Grab the Item
        if (collision.gameObject.CompareTag("Item")) {
            hasItem = true;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Mask")) {
            hasMask = true;
            Destroy(collision.gameObject);
            StartCoroutine(MaskPowerupCountdownCoroutine());
        }

        if (collision.gameObject.CompareTag("Trolley")) {
            hasTrolley = true;
            collision.transform.parent = transform;
        }

        //Exit Level
        if (collision.gameObject.CompareTag("Exit")) {
            if (hasItem) {
                gameManager.Win();
            } else {
                gameManager.ForgotItemWarning();
            }
        }

    }
}
