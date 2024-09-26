/** 
 * ANDRIX © 2024, based on the extOSC library (Vladimir Sigalkin), MIT License
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;
using extOSC;
using TMPro;

public class EquationOSCAndVFXController : MonoBehaviour
{

	//[Header("OSC Settings")]
	//public OSCTransmitter transmitter;

	[Header("OSC Receiver")]
	public OSCReceiver receiver;

	[Header("Local bindings")]
	public VisualEffect equationVFX;

	// Dashboard variables (the first 9 variables are displayed)
	public TextMeshProUGUI slot1;
	public TextMeshProUGUI slot2;
	public TextMeshProUGUI slot3;
	public TextMeshProUGUI slot4;
	public TextMeshProUGUI slot5;
	public TextMeshProUGUI slot6;
	public TextMeshProUGUI slot7;
	public TextMeshProUGUI slot8;
	public TextMeshProUGUI slot9;

	// Axes coordinates canvas
	public Canvas _coordinatesCanvas;
	private TextMeshProUGUI[] _coordinates;

	// Black overlay image
	public Image blackOverlayImg;

	// Equation image
	public Image equationImg;
	
	// Epileptic warning texts
	public TextMeshProUGUI epilepticSlot1;
	public TextMeshProUGUI epilepticSlot2;

	// Camera
	public RotAround cam;

	// Controls : Master system variables
	[Header("Master system variables")]
	
	[Range(0.0f, 1.0f)]
	public float masterIntensity = 0.0f;
	public PlayVariable MasterIntensity;
	
	[Range(0.0f, 1.0f)]
	public float equationIntensity = 0.0f;
	public PlayVariable EquationIntensity;
	
	[Range(0.0f, 1.0f)]
	public float turbulenceIntensity = 0.0f;
	public PlayVariable TurbulenceIntensity;

	// Controls : Equation variables
	[Header("Equation variables")]

	[Range(0.0f, 10.0f)]
	public float k0 = 1.0f;
	public PlayVariable K0;

	[Range(0.0f, 10.0f)]
	public float e0 = 1.0f;
	public PlayVariable E0;
	
	[Range(-3.0f, 3.0f)]
	public float lambda = 1.0f;
	public PlayVariable Lambda;
	
	[Range(-3.0f, 3.0f)]
	public float nu = 1.0f;
	public PlayVariable Nu;

	[Range(0.0f, 2.0f)]
	public float eta = 0.0f;
	public PlayVariable Eta;
	
	[Range(-3.0f, 3.0f)]
	public float a = 1.0f;
	public PlayVariable A;
	
	[Range(0.0f, 3.0f)]
	public float b = 1.0f;
	public PlayVariable B;
	
	[Range(0.0f, 3.0f)]
	public float alpha = 1.0f;
	public PlayVariable Alpha;
	
	[Range(0.0f, 3.0f)]
	public float beta = 1.0f;
	public PlayVariable Beta;
	
	private PlayVariable[] playVariables;
	private PlayVariable[] sortedVariables;

	// Controls : Visual controls
	[Header("Visual controls")]

	[Range(0.0f, 1.0f)]
	public float blackOverlayOpacity = 1.0f;
	private float lastBlackOverlayOpacity = 1.0f;

	[Range(0.0f, 1.0f)]
	public float epilepsyOpacity = 1.0f;
	private float lastEpilepsyOpacity = 1.0f;

	[Range(0.0f, 1.0f)]
	public float equationOpacity = 1.0f;
	private float lastEquationOpacity = 1.0f;

	[Range(0.0f, 1.0f)]
	public float axesOpacity = 1.0f;
	private float lastAxesOpacity = 1.0f;

	[Range(0.0f, 1.0f)]
	public float coordinatesOpacity = 1.0f;
	private float lastCoordinatesOpacity = 1.0f;

	[Range(0.0f, 1.0f)]
	public float dashboardOpacity = 1.0f;
	public bool dashboardMode = false;
	private bool lastDashboardMode = false;

	public bool togglePositiveCorner = true;
	private bool lastTogglePositionCorner = true;

	[Range(-1.0f, 1.0f)]
	public float camSpeed = 0.15f;
	private float lastCamSpeed = 0.1f;

	[Range(-400.0f, 400.0f)]
	public float camBaseAngle = 0.0f;
	private float lastCamBaseAngle = 0.0f;

	[Range(0.0f, 8.0f)]
	public float camRadius = 4.0f;
	private float lastCamRadius = 4.0f;


	protected void Start()
	{
		// Get all child coordinates from Coordinates Canvas
		_coordinates = _coordinatesCanvas.GetComponentsInChildren<TextMeshProUGUI>();

		// Store all play variables in an array
		K0 = new PlayVariable("k0", k0);
		E0 = new PlayVariable("E0", e0);
		Lambda = new PlayVariable("lambda", lambda);
		Nu = new PlayVariable("nu", nu);
		Eta = new PlayVariable("eta", eta);
		A = new PlayVariable("a", a);
		B = new PlayVariable("b", b);
		Alpha = new PlayVariable("alpha", alpha);
		Beta = new PlayVariable("beta", beta);

		MasterIntensity = new PlayVariable("masterIntensity", masterIntensity);
		EquationIntensity = new PlayVariable("equationIntensity", equationIntensity);
		TurbulenceIntensity = new PlayVariable("turbulenceIntensity", turbulenceIntensity);

		playVariables = new PlayVariable[12];
		playVariables[0] = K0;
		playVariables[1] = E0;
		playVariables[2] = Lambda;
		playVariables[3] = Nu;
		playVariables[4] = Eta;
		playVariables[5] = A;
		playVariables[6] = B;
		playVariables[7] = Alpha;
		playVariables[8] = Beta;
		playVariables[9] = MasterIntensity;
		playVariables[10] = EquationIntensity;
		playVariables[11] = TurbulenceIntensity;

		// Prepare array that will order displayed value based on last update time
		sortedVariables = new PlayVariable[12];
		playVariables.CopyTo(sortedVariables, 0);

		// Prepare OSC message reception
		receiver.Bind("/k0", HandleMessage);
		receiver.Bind("/e0", HandleMessage);
		receiver.Bind("/lambda", HandleMessage);
		receiver.Bind("/nu", HandleMessage);
		receiver.Bind("/eta", HandleMessage);
		receiver.Bind("/a", HandleMessage);
		receiver.Bind("/b", HandleMessage);
		receiver.Bind("/alpha", HandleMessage);
		receiver.Bind("/beta", HandleMessage);

		receiver.Bind("/overlay-opacity", HandleMessage);
		receiver.Bind("/epilepsy-opacity", HandleMessage);
		receiver.Bind("/equation-opacity", HandleMessage);
		receiver.Bind("/axes-opacity", HandleMessage);
		receiver.Bind("/coordinates-opacity", HandleMessage);
		receiver.Bind("/dashboard-opacity", HandleMessage);
		receiver.Bind("/cam-speed", HandleMessage);
		receiver.Bind("/cam-base-angle", HandleMessage);
		receiver.Bind("/cam-radius", HandleMessage);

		receiver.Bind("/dashboard-mode", HandleBoolMessage);
		receiver.Bind("/toggle-corner", HandleBoolMessage);

		receiver.Bind("/master", HandleMessage);
		receiver.Bind("/equation", HandleMessage);
		receiver.Bind("/turbulence", HandleMessage);
	}

	void Update()
	{
		Check();
	}

	// Values mapping (arriving [0, 1])
	private void HandleMessage(OSCMessage message)
	{
		string address = message.Address;
		float value = message.Values[0].FloatValue;

		if (address == "/k0") {	k0 = 10 * value; } // [0, 10]
		else if (address == "/e0") { e0 = 10 * value; } // [0, 10]
		else if (address == "/lambda") { lambda = -3 + 6 * value; } // [-3, 3]
		else if (address == "/nu") { nu = -3 + 6 * value; } // [-3, 3]
		else if (address == "/eta") { eta = 2 * value; } // [0, 2]
		else if (address == "/a") { a = -3 + 6 * value; } // [-3, 3]
		else if (address == "/b") { b = 3 * value; } // [0, 3]
		else if (address == "/alpha") { alpha = 3 * value; } // [0, 3]
		else if (address == "/beta") { beta = 3 * value; } // [0, 3]
		else if (address == "/overlay-opacity") { blackOverlayOpacity = value; }
		else if (address == "/epilepsy-opacity") { epilepsyOpacity = value; }
		else if (address == "/equation-opacity") { equationOpacity = value; }
		else if (address == "/axes-opacity") { axesOpacity = value; }
		else if (address == "/coordinates-opacity") { coordinatesOpacity = value; }
		else if (address == "/dashboard-opacity") { dashboardOpacity = value; }
		else if (address == "/cam-speed") { camSpeed = -1 + 2 * value; } // [-1, 1]
		else if (address == "/cam-base-angle") { camBaseAngle = -400 + 800 * value; } // [-400, 400]
		else if (address == "/cam-radius") { camRadius = 8 * value; } // [0, 8]
		else if (address == "/master") { masterIntensity = value; }
		else if (address == "/equation") { equationIntensity = value; }
		else if (address == "/turbulence") { turbulenceIntensity = value; }
		else
		{
			Debug.LogWarning("OSC address not recognized (Float section): " + address);
		}

	}

	private void HandleBoolMessage(OSCMessage message)
	{
		string address = message.Address;
		bool value = message.Values[0].BoolValue;

		if (address == "/dashboard-mode") {	dashboardMode = value;	}
		else if (address == "/toggle-corner") { togglePositiveCorner = value; }
		else
		{
			Debug.LogWarning("OSC address not recognized (Bool section): " + address);
		}
	}

	void Check()
	{
		// Check which variable, if any, has just been changed
		string variableName = "";
		float variableValue = 1.0f;

		if (k0 != playVariables[0].value) 		   					    { playVariables[0].update(k0); variableName = "k0"; variableValue = k0; }
		else if (e0 != playVariables[1].value)							{ playVariables[1].update(e0); variableName = "E0"; variableValue = e0; }
		else if (lambda != playVariables[2].value) 						{ playVariables[2].update(lambda); variableName = "lambda"; variableValue = lambda; }
		else if (nu != playVariables[3].value) 		 					{ playVariables[3].update(nu); variableName = "nu"; variableValue = nu; }
		else if (eta != playVariables[4].value) 				 		{ playVariables[4].update(eta); variableName = "eta"; variableValue = eta; }
		else if (a != playVariables[5].value) 					 		{ playVariables[5].update(a); variableName = "a"; variableValue = a; }
		else if (b != playVariables[6].value) 							{ playVariables[6].update(b); variableName = "b"; variableValue = b; }
		else if (alpha != playVariables[7].value) 	 					{ playVariables[7].update(alpha); variableName = "alpha"; variableValue = alpha; }
		else if (beta != playVariables[8].value) 	 					{ playVariables[8].update(beta); variableName = "beta"; variableValue = beta; }

		// Smooth with a square law for master inputs (store raw in PlayVariable object, send smoothed value to VFX)
		else if (masterIntensity != playVariables[9].value) 			{ playVariables[9].update(masterIntensity); variableName = "masterIntensity"; variableValue = Mathf.Pow(masterIntensity, 2.0f); }
		else if (equationIntensity != playVariables[10].value) 			{ playVariables[10].update(equationIntensity); variableName = "equationIntensity"; variableValue = Mathf.Pow(equationIntensity, 2.0f); }
		else if (turbulenceIntensity != playVariables[11].value) 		{ playVariables[11].update(turbulenceIntensity); variableName = "turbulenceIntensity"; variableValue = Mathf.Pow(turbulenceIntensity, 2.0f); }

		// Handle variable update if one value changed
		if (variableName.Length > 0)
		{
			// Send OSC message
			/*
			string address = "/project/param/1/value";// + variableName;
			OSCMessage message = new OSCMessage(address);
			message.AddValue(OSCValue.Float(variableValue));
			transmitter.Send(message);
			*/

			// Change local VFX value
			equationVFX.SetFloat(variableName, variableValue);

			// Update dashboard values (order by last update time)
			Array.Sort<PlayVariable>(sortedVariables);

			UpdateDashboardText();
		}

		// Update positive corner toggle if changed
		if (lastTogglePositionCorner != togglePositiveCorner)
		{
			equationVFX.SetBool("togglePositiveCorner", togglePositiveCorner);
			lastTogglePositionCorner = togglePositiveCorner;
		}

		// Handle update if one dashboard mode toggles changed
		if (lastDashboardMode != dashboardMode)
		{
			lastDashboardMode = dashboardMode;

			UpdateDashboardText();
		}

		// Update opacities if one value changed
		if (lastBlackOverlayOpacity != blackOverlayOpacity || lastAxesOpacity != axesOpacity || lastCoordinatesOpacity != coordinatesOpacity || lastEquationOpacity != equationOpacity || lastEpilepsyOpacity != epilepsyOpacity)
		{
			UpdateOpacities();
		}
		
		// Update camera variables if one value changed
		if (lastCamSpeed != camSpeed ||	lastCamBaseAngle != camBaseAngle ||	lastCamRadius != camRadius)
		{
			UpdateCamera();
		}

		// Always update dashboard text opacities
		UpdateDashboardOpacities();

	}

	void UpdateOpacities()
	{
		// Update black overlay opacity (smoothed with ~cubic root power law)
		blackOverlayImg.color = new Color(1, 1, 1, Mathf.Pow(blackOverlayOpacity, 0.3f));

		// Update VFX axes opacity (smoothed with square power law)
		equationVFX.SetFloat("axesOpacity", Mathf.Pow(axesOpacity, 2.0f));
		
		// Update coordinates opacity (smoothed with square power law)
		foreach (TextMeshProUGUI _coord in _coordinates)
		{
			_coord.color = new Color(1, 1, 1, Mathf.Pow(coordinatesOpacity, 2.0f));
		}

		// Update equation image opacity (smoothed with square power law)
		equationImg.color = new Color(1, 1, 1, Mathf.Pow(equationOpacity, 2.0f));
		
		// Update epilepsy warning opacity (smoothed with square power law)
		epilepticSlot1.color = new Color(1, 1, 1, Mathf.Pow(epilepsyOpacity, 2.0f));
		epilepticSlot2.color = new Color(1, 1, 1, Mathf.Pow(epilepsyOpacity, 2.0f));

		// Update value buffers
		lastBlackOverlayOpacity = blackOverlayOpacity;
		lastAxesOpacity = axesOpacity;
		lastCoordinatesOpacity = coordinatesOpacity;
		lastEquationOpacity = equationOpacity;
		lastEpilepsyOpacity = epilepsyOpacity;
	}

	void UpdateCamera()
	{
		// Update camera variables
		float smoothedCamSpeed = Mathf.Sign(camSpeed) * Mathf.Pow(Mathf.Abs(camSpeed), 3.0f);
		cam.revolutionSpeed = smoothedCamSpeed;
		cam.baseRotAngleY = camBaseAngle;
		cam.radius = camRadius;

		// Update value buffers
		lastCamSpeed = camSpeed;
		lastCamBaseAngle = camBaseAngle;
		lastCamRadius = camRadius;
	}

	void UpdateDashboardText()
	{
		// Update text content (dashboardMode : true => fading with time and reordered (last changed variable on top), false => always displayed and in the same order)
		if (dashboardMode)
		{
			slot1.text = sortedVariables[0].getText();
			slot2.text = sortedVariables[1].getText();
			slot3.text = sortedVariables[2].getText();
			slot4.text = sortedVariables[3].getText();
			slot5.text = sortedVariables[4].getText();
			slot6.text = sortedVariables[5].getText();
			slot7.text = sortedVariables[6].getText();
			slot8.text = sortedVariables[7].getText();
			slot9.text = sortedVariables[8].getText();
		}
		else
		{
			slot1.text = playVariables[0].getText();
			slot2.text = playVariables[1].getText();
			slot3.text = playVariables[2].getText();
			slot4.text = playVariables[3].getText();
			slot5.text = playVariables[4].getText();
			slot6.text = playVariables[5].getText();
			slot7.text = playVariables[6].getText();
			slot8.text = playVariables[7].getText();
			slot9.text = playVariables[8].getText();
		}

	}

	void UpdateDashboardOpacities()
	{
		// Compute smoothed dashboard opacity (square power law for smoother control)
		float dashOpa = Mathf.Pow(dashboardOpacity, 2.0f);

		// dashboardMode : true => fading with time and reordered (last changed variable on top), false => always displayed and in the same order
		if (dashboardMode)
		{
			slot1.color = new Color(1, 1, 1, sortedVariables[0].getOpacity() * dashOpa);
			slot2.color = new Color(1, 1, 1, sortedVariables[1].getOpacity() * dashOpa);
			slot3.color = new Color(1, 1, 1, sortedVariables[2].getOpacity() * dashOpa);
			slot4.color = new Color(1, 1, 1, sortedVariables[3].getOpacity() * dashOpa);
			slot5.color = new Color(1, 1, 1, sortedVariables[4].getOpacity() * dashOpa);
			slot6.color = new Color(1, 1, 1, sortedVariables[5].getOpacity() * dashOpa);
			slot7.color = new Color(1, 1, 1, sortedVariables[6].getOpacity() * dashOpa);
			slot8.color = new Color(1, 1, 1, sortedVariables[7].getOpacity() * dashOpa);
			slot9.color = new Color(1, 1, 1, sortedVariables[8].getOpacity() * dashOpa);
		}
		else
		{
			slot1.color = new Color(1, 1, 1, 1 * dashOpa);
			slot2.color = new Color(1, 1, 1, 1 * dashOpa);
			slot3.color = new Color(1, 1, 1, 1 * dashOpa);
			slot4.color = new Color(1, 1, 1, 1 * dashOpa);
			slot5.color = new Color(1, 1, 1, 1 * dashOpa);
			slot6.color = new Color(1, 1, 1, 1 * dashOpa);
			slot7.color = new Color(1, 1, 1, 1 * dashOpa);
			slot8.color = new Color(1, 1, 1, 1 * dashOpa);
			slot9.color = new Color(1, 1, 1, 1 * dashOpa);
		}

	}
}
