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
    //private float angleX = 0;
    private float angleY = 0;
    //private float angleZ = 0;

    public float radius = 5.0f;
    public float inertia = 2.0f;
    private float hamperedRadius = 5.0f;

    private float t = 0;

    void Start()
    {
        
    }

    void Update()
    {

        // Update hampered radius with some inertia
        hamperedRadius += (radius - hamperedRadius) / inertia;

        // Update rotation
        t += revolutionSpeed * Time.deltaTime;
        angleY = baseRotAngleY + t;
        Vector3 offset = new Vector3(hamperedRadius * Mathf.Cos(angleY), 0, hamperedRadius * Mathf.Sin(angleY));

        gameObject.transform.localPosition = origin.position + offset;
    }

}
