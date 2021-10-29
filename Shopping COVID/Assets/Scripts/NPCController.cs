using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour {
    //public Camera cam;
    public NavMeshAgent agent;
    public float range = 23.0f;
    public GameObject enemyPrefab;
    public GameObject assetModel; //TODO convert NPC/Enemy to Scriptable objects...
    private GameManager gameManager;
    [SerializeField]
    private float walkingSpeed = 6;
    [SerializeField]
    private float runningSpeed = 15;
    private float speed = 6;

    private Animator animator;

    private int mapMargin = 4;
    private int mapHeight = 50;
    private int mapWidth = 50;


    protected void Start() {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(WaitForNextMove());
    }

    void Update() {
        animator.SetFloat("Speed_f", agent.velocity.magnitude);
        agent.speed = speed;
    }

    private void MoveAgent() {
        float posX = Random.Range(mapMargin, mapHeight - mapMargin);
        float posZ = Random.Range(mapMargin, mapWidth - mapMargin);
        Vector3 destination = new Vector3(posX, 0, posZ);
        agent.SetDestination(destination);
    }

    protected IEnumerator WaitForNextMove() {
        yield return new WaitForSeconds(1);
        MoveAgent();
        yield return new WaitForSeconds(Random.Range(5, 10));
        speed = walkingSpeed;
        StartCoroutine(WaitForNextMove());
    }

    protected IEnumerator RunScared() {
        speed = runningSpeed;
        MoveAgent();
        Debug.Log("NPC Running!!!");
        yield return new WaitForSeconds(5);
    }

    public void Infect() {
        if (gameManager != null && gameManager.IsGameActive) {
            Debug.Log("NPC Infected!!!");
            GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            if (enemy.transform.childCount == 1) {
                enemy.transform.GetChild(0).gameObject.SetActive(false);
            }
            assetModel.transform.parent = enemy.transform;
            Destroy(gameObject);

            NPCController npc = enemy.gameObject.GetComponent<NPCController>();
            StartCoroutine(npc.RunScared());

        }
    }

    public void SetMapDimensions(int mapHeight, int mapWidth) {
        this.mapHeight = mapHeight;
        this.mapWidth = mapWidth;
    }
    public void Scare() {
        StartCoroutine(RunScared());
    }
}