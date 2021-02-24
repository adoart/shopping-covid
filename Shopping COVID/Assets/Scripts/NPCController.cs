using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    //public Camera cam;
    public NavMeshAgent agent;
    public float range = 23.0f;
    public GameObject enemyPrefab;
    public GameObject assetModel; //TODO convert NPC/Enemy to Scriptable objects...
    private GameManager gameManager;

    private Animator animator;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(WaitForNextMove());
    }

    void Update() {
        animator.SetFloat("Speed_f", agent.velocity.magnitude);
    }

    private void MoveAgent() {
        float posX = Random.Range(-range, range);
        float posZ = Random.Range(-range, range);
        Vector3 destination = new Vector3(posX, 0, posZ);
        agent.SetDestination(destination);
    }

    protected IEnumerator WaitForNextMove() {
        MoveAgent();
        yield return new WaitForSeconds(Random.Range(5,10));
        StartCoroutine(WaitForNextMove());
    }

    public void Infect() {
        if (gameManager != null && gameManager.IsGameActive) {
            Debug.Log("NPC Infected!!!");
            GameObject enemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
            assetModel.transform.parent = enemy.transform;
            Destroy(gameObject);
        }
    }
}
