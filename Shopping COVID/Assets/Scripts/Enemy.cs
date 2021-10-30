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

    private void Cough() {
        //TODO animation
        // Debug.Log("Coughed!!!");
        Collider[] colliders = new Collider[30];
        Physics.OverlapSphereNonAlloc(transform.position, coughingRadius, colliders);
        IEnumerable<Collider> queryColliders =
            from collider in colliders
            where collider != null && collider.gameObject.CompareTag("NPC")
            select collider;
        foreach (Collider other in queryColliders) {
            NPCController npc = other.gameObject.GetComponent<NPCController>();
            npc.Scare();
        }
    }

    #region Debug Cough area/radius

    /// <summary>
    /// Debug Cough area/radius block
    /// </summary>
    public int Segments = 32;
    public Color Color = Color.blue;

    private void OnDrawGizmos() {
        DrawEllipse(transform.position, transform.up, transform.forward, coughingRadius * transform.localScale.x,
            coughingRadius * transform.localScale.y, Segments, Color);
    }

    private void DrawEllipse(Vector3 pos, Vector3 forward, Vector3 up, float radiusX, float radiusY, int segments,
        Color color, float duration = 0) {
        float angle = 0f;
        Quaternion rot = Quaternion.LookRotation(forward, up);
        Vector3 lastPoint = Vector3.zero;
        Vector3 thisPoint = Vector3.zero;

        for (int i = 0; i < segments + 1; i++) {
            thisPoint.x = Mathf.Sin(Mathf.Deg2Rad * angle) * radiusX;
            thisPoint.y = Mathf.Cos(Mathf.Deg2Rad * angle) * radiusY;

            if (i > 0) {
                Debug.DrawLine(rot * lastPoint + pos, rot * thisPoint + pos, color, duration);
            }

            lastPoint = thisPoint;
            angle += 360f / segments;
        }
    }

    #endregion
}