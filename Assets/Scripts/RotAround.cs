/**
 * Alex Andrix © 2023-2024
 * This script animates a game object's revolution
 */

using UnityEngine;

public class RotAround : MonoBehaviour
{
    public float revolutionSpeed = 0.5f;
    public Transform origin;
    public float baseRotAngleY = 0;
    public float radius = 5.0f;

    private float angleX = 0;
    private float angleY = 0;
    private float angleZ = 0;
    private float t = 0;

    void Start()
    {
        
    }

    void Update()
    {

        // Update rotation
        t += revolutionSpeed * Time.deltaTime;
        angleY = baseRotAngleY + t;
        Vector3 offset = new Vector3(radius * Mathf.Cos(angleY), 0, radius * Mathf.Sin(angleY));

        gameObject.transform.localPosition = origin.position + offset;
    }

}
