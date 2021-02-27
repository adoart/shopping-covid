using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAnimation : MonoBehaviour
{
    [SerializeField]private float rotationSpeed;
    [SerializeField]private float movementY;
    [SerializeField] private float movementSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, Mathf.Sin(Time.time * movementSpeed) * movementY, 0, Space.Self);
        transform.Rotate(Vector3.up, Time.deltaTime * rotationSpeed, Space.Self);
    }
}
