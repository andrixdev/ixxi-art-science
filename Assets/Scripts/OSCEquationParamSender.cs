/** 
 * ANDRIX © 2024, based on the extOSC library (Vladimir Sigalkin), MIT License
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using extOSC;
using TMPro;

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

	[Range(-3.0f, 3.0f)]
	public float nu = 1.0f;
	private float lastNu = 1.0f;

	[Range(-3.0f, 3.0f)]
	public float lambda = 1.0f;
	private float lastLambda = 1.0f;

	[Range(0.0f, 3.0f)]
	public float eta = 0.0f;
	private float lastEta = 0.0f;

	[Range(-3.0f, 3.0f)]
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

	[Range(0.0f, 5.0f)]
	public float turbulence = 0.0f;
	private float lastTurbulence = 0.0f;

	[Range(0.0f, 5.0f)]
	public float incrementSpeed = 1.5f;
	private float lastIncrementSpeed = 1.5f;

	[Range(0.0f, 1.0f)]
	public float opacity = 1.0f;

	// Variables displayed text
	public TextMeshProUGUI _E0;
	public TextMeshProUGUI _E4;
	public TextMeshProUGUI _nu;
	public TextMeshProUGUI _lambda;
	public TextMeshProUGUI _k0;
	public TextMeshProUGUI _eta;

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
		else if (turbulence != lastTurbulence) { lastTurbulence = turbulence; variableName = "turbulence"; variableValue = turbulence; }
		else if (incrementSpeed != lastIncrementSpeed) { lastIncrementSpeed = incrementSpeed; variableName = "incrementSpeed"; variableValue = incrementSpeed; }

		if (variableName.Length > 0)
		{
			// Send OSC message
			string address = "/project/param/1/value";// + variableName;
			OSCMessage message = new OSCMessage(address);
			message.AddValue(OSCValue.Float(variableValue));
			transmitter.Send(message);

			// Change local VFX value
			equationVFX.SetFloat(variableName, variableValue);

			// Update display TextMeshPro values
			_E0.text = "E0 = " + E0.ToString();
			_E4.text = "E4 = " + E4.ToString();
			_nu.text = "nu = " + nu.ToString();
			_lambda.text = "lambda = " + lambda.ToString();
			_k0.text = "k0 = " + k0.ToString();
			_eta.text = "eta = " + eta.ToString();

		}

		// Update colors (opacity)
		_E0.color = new Color(1, 1, 1, opacity);
		_E4.color = new Color(1, 1, 1, opacity);
		_nu.color = new Color(1, 1, 1, opacity);
		_lambda.color = new Color(1, 1, 1, opacity);
		_k0.color = new Color(1, 1, 1, opacity);
		_eta.color = new Color(1, 1, 1, opacity);

	}

}
