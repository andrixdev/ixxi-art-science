/**
 * Alex Andrix © 2023-2026
 * This script animates a game object's revolution
 */

using UnityEngine;

public class RotGently : MonoBehaviour
{
    public float revolutionSpeed = 0.5f;
    public Transform startPoint;
    public Transform endPoint;

    public float baseRotAngleY = 0;
    //private float angleX = 0;
    private float angleY = 0;
    //private float angleZ = 0;

    public float motionLerpValue = 0.0f; // Start point, end point is 1.0f
    public float inertia = 2.0f;
    private float hamperedMotionLerpValue = 0.0f;

    private float t = 0;

    void Start()
    {
        
    }

    void Update()
    {

        // Update motion lerp value with some inertia
        hamperedMotionLerpValue += (motionLerpValue - hamperedMotionLerpValue) / inertia;

        // Update rotation
        t += revolutionSpeed * Time.deltaTime;
        angleY = baseRotAngleY + t;

        gameObject.transform.localPosition = Vector3.Lerp(startPoint.position, endPoint.position, hamperedMotionLerpValue);
        gameObject.transform.localRotation = Quaternion.Euler(0f, angleY * 180.0f / Mathf.PI, 0f);
    }

}
