using UnityEngine;
using FMODUnity;

public class FMODParamStream : MonoBehaviour
{
    public StudioEventEmitter emitter;
    public Transform handleTransform;

    void OnEnable()
    {
        if (!emitter)
        {
            Debug.Log("Error: missing FMOD Studio Event Emitter.");
        }
    }
    void Update()
    {
        // Update every frame
        float value = Mathf.Clamp(handleTransform.position.y, 0, 3.0f);
        emitter.SetParameter("Frequency", value);
        Debug.Log(value);
    }
}
