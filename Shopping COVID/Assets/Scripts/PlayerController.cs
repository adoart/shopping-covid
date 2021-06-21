using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {
    #region Events

    public event Action HitByEnemy;

    public event Action<GameObject> PickedUpItem;

    public event Action TriedToExit;

    #endregion

    public Camera cam;
    public NavMeshAgent agent;
    private Animator playerAnimator;
    private GameManager gameManager;
    private AudioSource audioSource;
    [SerializeField] private GameObject powerupIndicator;
    [SerializeField] private AudioClip powerupPickupSound;
    [SerializeField] private AudioClip itemPickupSound;
    [SerializeField] private GameObject forgotTextBubble;
    [SerializeField] private Vector3 forgotBubbleOffset;

    private bool hasMask;

    void Start() {
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
            forgotTextBubble.transform.position = cam.WorldToScreenPoint(transform.position + forgotBubbleOffset);
        }
    }

    public void LooseLife() {
        if (HitByEnemy != null && !hasMask) {
            HitByEnemy();
        }
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

    private void OnCollisionEnter(Collision collision) {
        //Grab the Item
        if (collision.gameObject.CompareTag("Item")) {
            Transform parent = collision.gameObject.transform.parent;
            if (parent == null) {
                parent = collision.transform;
            }
            if (PickedUpItem != null) {
                PickedUpItem(parent.gameObject);
            }
            audioSource.PlayOneShot(itemPickupSound);
            Destroy(parent.gameObject);
        }

        if (collision.gameObject.CompareTag("Mask")) {
            audioSource.PlayOneShot(powerupPickupSound);
            Destroy(collision.gameObject);
            StartCoroutine(MaskPowerupCountdownCoroutine());
        }

        if (collision.gameObject.CompareTag("Trolley")) {
            audioSource.PlayOneShot(powerupPickupSound);
            collision.transform.parent = transform;
            collision.transform.rotation = transform.rotation;
            collision.transform.position = transform.position;
        }

        //Exit Level
        if (collision.gameObject.CompareTag("Exit")) {
            if (TriedToExit != null) {
                TriedToExit();
            }
        }
    }
}