/**
 * Alex Andrix © 2023-2024
 * This script animates a game object's linear rotation
 */

using UnityEngine;

public class GrowthRotAnim : MonoBehaviour
{
    public float rotationSpeed = 0.02f;
    
    void Start()
    {
        
    }

    void Update()
    {

        float deltaAngle = Time.deltaTime * rotationSpeed;
        gameObject.transform.Rotate(0, deltaAngle, deltaAngle / 3, Space.Self);
    }

}
