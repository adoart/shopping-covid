using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : NPCController {
    [SerializeField]
    private float coughingRadius = 10.0f;

    new void Start() {
        base.Start();
        StartCoroutine(WaitForNextCough());
    }
    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.gameObject.CompareTag("Player")) {
            Debug.Log("Collided with Player!!!");
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            player.LooseLife();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("NPC")) {
            NPCController npc = other.gameObject.GetComponent<NPCController>();
            npc.Infect();
        }
    }

    protected IEnumerator WaitForNextCough() {
        yield return new WaitForSeconds(1);
        Cough();
        yield return new WaitForSeconds(Random.Range(5, 10));
        StartCoroutine(WaitForNextCough());
    }

    public void Cough() {
        //TODO animation
        // Debug.Log("Coughed!!!");
        Collider[] colliders = new Collider[15];
        Physics.OverlapSphereNonAlloc(transform.position, coughingRadius, colliders);
        IEnumerable<Collider> queryColliders =
            from collider in colliders
            where collider != null && collider.gameObject.CompareTag("NPC")
            select collider;
        foreach (Collider other in queryColliders) {
            Debug.Log("Coughing Scared!!!");
            NPCController npc = other.gameObject.GetComponent<NPCController>();
            npc.Scare();
        }
    }
}