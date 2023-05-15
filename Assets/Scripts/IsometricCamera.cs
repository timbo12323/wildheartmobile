using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    public Transform target;  // The target object to follow
    public float distance = 10f;  // Distance from the target
    public float height = 10f;  // Height of the camera above the target
    public float rotationSpeed = 5f;  // Speed of camera rotation

    private float currentRotation = 45f;  // Initial camera rotation

    void LateUpdate()
    {
        if (target == null)
            return;

        // Calculate the desired position based on the target's position, distance, and height
        Vector3 desiredPosition = target.position - Quaternion.Euler(0f, currentRotation, 0f) * Vector3.forward * distance;
        desiredPosition.y = target.position.y + height;

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * rotationSpeed);

        // Rotate the camera around the target based on user input
        if (Input.GetKey(KeyCode.Q))
        {
            currentRotation -= rotationSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            currentRotation += rotationSpeed * Time.deltaTime;
        }

        // Ensure the camera is always looking at the target
        transform.LookAt(target);
    }
}