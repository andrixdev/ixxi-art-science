/**
 * Alex Andrix © 2023-2024
 * This script animates an oscillating linear traveling
 */

using UnityEngine;

public class GrowthPosAnim : MonoBehaviour
{
    public float freq = 1.0f;
    public float amplitude = 2.0f;
    public float centerZ = -5.0f;
    private float angle = 0;

    void Start()
    {
        
    }

    void Update()
    {

        // Update position
        angle += freq * Time.deltaTime;
        float z = centerZ + amplitude * Mathf.Sin(angle);
        Vector3 newPos = new Vector3(0, 0, z);
        gameObject.transform.localPosition = newPos;
        
    }

}
