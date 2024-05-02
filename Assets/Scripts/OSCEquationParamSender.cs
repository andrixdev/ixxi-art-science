/** 
 * ANDRIX © 2024, based on the extOSC library (Vladimir Sigalkin), MIT License
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using extOSC;

public class OSCEquationParamSender : MonoBehaviour
{

	[Header("OSC Settings")]
	public OSCTransmitter transmitter;
	public VisualEffect equationVFX;

	// Equation variables
	[Range(0.0f, 10.0f)]
	public float k0 = 1.0f;
	private float lastK0 = 1.0f;

	[Range(0.0f, 10.0f)]
	public float E0 = 1.0f;
	private float lastE0 = 1.0f;

	[Range(0.0f, 10.0f)]
	public float E4 = 1.0f;
	private float lastE4 = 1.0f;

	[Range(0.0f, 3.0f)]
	public float nu = 1.0f;
	private float lastNu = 1.0f;

	[Range(0.0f, 3.0f)]
	public float lambda = 1.0f;
	private float lastLambda = 1.0f;

	[Range(0.0f, 3.0f)]
	public float eta = 0.0f;
	private float lastEta = 0.0f;

	[Range(0.0f, 3.0f)]
	public float a = 1.0f;
	private float lastA = 1.0f;

	[Range(0.0f, 3.0f)]
	public float b = 1.0f;
	private float lastB = 1.0f;

	[Range(0.0f, 3.0f)]
	public float alpha = 1.0f;
	private float lastAlpha = 1.0f;

	[Range(0.0f, 3.0f)]
	public float beta = 1.0f;
	private float lastBeta = 1.0f;

	protected void Start()
	{
		
	}

	void Update()
	{
		Check();
	}

	void Check()
	{
		string variableName = "";
		float variableValue = 1;

		if (k0 != lastK0) { lastK0 = k0; variableName = "k0"; variableValue = k0; }
		else if (E0 != lastE0) { lastE0 = E0; variableName = "E0"; variableValue = E0; }
		else if (E4 != lastE4) { lastE4 = E4; variableName = "E4"; variableValue = E4; }
		else if (nu != lastNu) { lastNu = nu; variableName = "nu"; variableValue = nu; }
		else if (lambda != lastLambda) { lastLambda = lambda; variableName = "lambda"; variableValue = lambda; }
		else if (eta != lastEta) { lastEta = eta; variableName = "eta"; variableValue = eta; }
		else if (a != lastA) { lastA = a; variableName = "a"; variableValue = a; }
		else if (b != lastB) { lastB = b; variableName = "b"; variableValue = b; }
		else if (alpha != lastAlpha) { lastAlpha = alpha; variableName = "alpha"; variableValue = alpha; }
		else if (beta != lastBeta) { lastBeta = beta; variableName = "beta"; variableValue = beta; }

		if (variableName.Length > 0)
		{
			// Send OSC message
			string address = "/project/param/1/value";// + variableName;
			OSCMessage message = new OSCMessage(address);
			message.AddValue(OSCValue.Float(variableValue));
			transmitter.Send(message);

			// Change local VFX value
			equationVFX.SetFloat(variableName, variableValue);
		}
	}

}
