/** 
 * ANDRIX © 2023, based on the extOSC library (Vladimir Sigalkin), MIT License
 *
 * EEG OSC messages include /eeg, /ppg, /drlref and /acc
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using extOSC;

public class EEGMessageReceiver : MonoBehaviour
{

	[Header("OSC Settings")]
	public OSCReceiver Receiver;
	public VisualEffect thoughtVFX;

    private int stepCount = 0;
	private float[] totals = new float[50];
    private float average;

	protected void Start()
	{
		// Setup OSC bindings
		Receiver.enabled = true;
		Receiver.Bind("/eeg", ReceivedEEGMessage);
	}

	private void ReceivedEEGMessage(OSCMessage message)
	{
		float v0 = message.Values[0].FloatValue;
		float v1 = message.Values[1].FloatValue;
		float v2 = message.Values[2].FloatValue;
		float v3 = message.Values[3].FloatValue;

        float val = v0 + v1 + v2 + v3;

		totals[stepCount % 50] = val;

        stepCount++;

		// Get average
        average = 0;
		for (int j = 0; j < 50; j++)
		{
			average += totals[j];
		}
		average /= 50;

		// Get signal power
		float power = 0;
		power = Mathf.Pow(val - average, 2.0f);
		power /= 1000;

        if (stepCount % 50 == 0)
        {
            Debug.Log("<color=cyan>" + power + "</color>");
        	thoughtVFX.SetFloat("eeg", power);
        }
	}

}
