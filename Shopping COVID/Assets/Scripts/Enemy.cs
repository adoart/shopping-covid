using UnityEngine;

public class Enemy : NPCController {
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
}