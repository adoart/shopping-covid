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
    [SerializeField] private GameObject destinationIndicator;
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
        destinationIndicator.SetActive(false);
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

    public void Warp(Vector3 position) {
        agent.Warp(position);
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
                destinationIndicator.SetActive(true);
                agent.SetDestination(hit.point);
                destinationIndicator.transform.position = new Vector3(hit.point.x, 0.2f, hit.point.z);
            }
        }
        if (!agent.hasPath && !agent.pathPending) {
            destinationIndicator.SetActive(false);
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
            var trans = transform;
            collision.transform.parent = trans;
            collision.transform.rotation = trans.rotation;
            collision.transform.position = trans.position;
        }

        //Exit Level
        if (collision.gameObject.CompareTag("Exit")) {
            if (TriedToExit != null) {
                TriedToExit();
            }
        }
    }
}