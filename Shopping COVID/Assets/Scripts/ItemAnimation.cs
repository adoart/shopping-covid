using UnityEngine;

public class ItemAnimation : MonoBehaviour {
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float movementY = 0.002f;
    [SerializeField] private float movementSpeed = 1.0f;

    // Update is called once per frame
    void Update() {
        transform.Translate(0, Mathf.Sin(Time.time * movementSpeed) * movementY, 0, Space.Self);
        transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed, Space.Self);
    }
}