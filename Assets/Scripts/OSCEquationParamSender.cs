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

public class OSCEquationParamSender : MonoBehaviour
{

	[Header("OSC Settings")]
	public OSCTransmitter transmitter;

	[Header("Local bindings")]
	public VisualEffect equationVFX;

	// Dashboard variables
	public TextMeshProUGUI slot1;
	public TextMeshProUGUI slot2;
	public TextMeshProUGUI slot3;
	public TextMeshProUGUI slot4;
	public TextMeshProUGUI slot5;
	public TextMeshProUGUI slot6;
	public TextMeshProUGUI slot7;
	public TextMeshProUGUI slot8;
	public TextMeshProUGUI slot9;
	public TextMeshProUGUI slot10;

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

	// Equation variables
	[Header("Equation variables")]
	[Range(0.0f, 10.0f)]
	public float k0 = 1.0f;
	public PlayVariable K0;
	//private float lastK0 = 1.0f;
	//private float k0time = 0.0f;

	[Range(0.0f, 10.0f)]
	public float e0 = 1.0f;
	public PlayVariable E0;
	//private float lastE0 = 1.0f;
	//private float E0time = 0.0f;

	[Range(-3.0f, 3.0f)]
	public float lambda = 1.0f;
	public PlayVariable Lambda;
	// private float lastLambda = 1.0f;
	// private float lambdaTime = 0.0f;

	[Range(-3.0f, 3.0f)]
	public float nu = 1.0f;
	
	public PlayVariable Nu;
	// private float lastNu = 1.0f;
	// private float nuTime = 0.0f;

	[Range(0.0f, 2.0f)]
	public float eta = 0.0f;
	public PlayVariable Eta;
	// private float lastEta = 0.0f;
	// private float etaTime = 0.0f;

	[Range(-3.0f, 3.0f)]
	public float a = 1.0f;
	public PlayVariable A;
	// private float lastA = 1.0f;
	// private float aTime = 0.0f;

	[Range(0.0f, 3.0f)]
	public float b = 1.0f;
	public PlayVariable B;
	// private float lastB = 1.0f;
	// private float bTime = 0.0f;

	[Range(0.0f, 3.0f)]
	public float alpha = 1.0f;
	public PlayVariable Alpha;
	// private float lastAlpha = 1.0f;
	// private float alphaTime = 0.0f;

	[Range(0.0f, 3.0f)]
	public float beta = 1.0f;
	public PlayVariable Beta;
	// private float lastBeta = 1.0f;
	// private float betaTime = 0.0f;

	[Range(0.0f, 5.0f)]
	public float turbulence = 0.0f;
	public PlayVariable Turbulence;
	// private float lastTurbulence = 0.0f;
	// private float turbulenceTime = 0.0f;

	[Range(0.0f, 5.0f)]
	public float incrementSpeed = 1.5f;
	// private float lastIncrementSpeed = 1.5f;

	private PlayVariable[] playVariables;
	private PlayVariable[] sortedVariables;


	[Header("Visual controls")]
	[Range(0.0f, 1.0f)]
	public float blackOverlayOpacity = 1.0f;
	private float lastBlackOverlayOpacity = 1.0f;

	[Range(0.0f, 1.0f)]
	public float axesOpacity = 1.0f;
	private float lastAxesOpacity = 1.0f;

	[Range(0.0f, 1.0f)]
	public float coordinatesOpacity = 1.0f;
	private float lastCoordinatesOpacity = 1.0f;

	[Range(0.0f, 1.0f)]
	public float dashboardOpacity = 1.0f;
	public bool dashboardOrderToggle = false;
	public bool dashboardFadingToggle = false;
	private bool lastDashboardOrderToggle = false;
	private bool lastDashboardFadingToggle = false;

	[Range(0.0f, 1.0f)]
	public float equationOpacity = 1.0f;
	private float lastEquationOpacity = 1.0f;

	[Range(0.0f, 1.0f)]
	public float epilepsyOpacity = 1.0f;
	private float lastEpilepsyOpacity = 1.0f;

	[Range(-0.5f, 0.5f)]
	public float camSpeed = 0.07f;
	private float lastCamSpeed = 0.07f;

	[Range(-720.0f, 720.0f)]
	public float camBaseAngle = 0.0f;
	private float lastCamBaseAngle = 0.0f;

	[Range(-2.0f, 8.0f)]
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
		Turbulence = new PlayVariable("turbulence", turbulence);

		playVariables = new PlayVariable[10];
		playVariables[0] = K0;
		playVariables[1] = E0;
		playVariables[2] = Lambda;
		playVariables[3] = Nu;
		playVariables[4] = Eta;
		playVariables[5] = A;
		playVariables[6] = B;
		playVariables[7] = Alpha;
		playVariables[8] = Beta;
		playVariables[9] = Turbulence;

		// Prepare array that will order displayed value based on last update time
		sortedVariables = new PlayVariable[10];
		playVariables.CopyTo(sortedVariables, 0);
	}

	void Update()
	{
		Check();
	}

	void Check()
	{
		string variableName = "";
		float variableValue = 1.0f;

		if (k0 != playVariables[0].value) 		   		    { playVariables[0].update(k0); variableName = "k0"; variableValue = k0; }
		else if (e0 != playVariables[1].value)				{ playVariables[1].update(e0); variableName = "E0"; variableValue = e0; }
		else if (lambda != playVariables[2].value) 			{ playVariables[2].update(lambda); variableName = "lambda"; variableValue = lambda; }
		else if (nu != playVariables[3].value) 		 		{ playVariables[3].update(nu); variableName = "nu"; variableValue = nu; }
		else if (eta != playVariables[4].value) 		 	{ playVariables[4].update(eta); variableName = "eta"; variableValue = eta; }
		else if (a != playVariables[5].value) 			 	{ playVariables[5].update(a); variableName = "a"; variableValue = a; }
		else if (b != playVariables[6].value) 				{ playVariables[6].update(b); variableName = "b"; variableValue = b; }
		else if (alpha != playVariables[7].value) 	 		{ playVariables[7].update(alpha); variableName = "alpha"; variableValue = alpha; }
		else if (beta != playVariables[8].value) 	 		{ playVariables[8].update(beta); variableName = "beta"; variableValue = beta; }
		else if (turbulence != playVariables[9].value) 		{ playVariables[9].update(turbulence); variableName = "turbulence"; variableValue = turbulence; }

		// Handle variable update if one value changed
		if (variableName.Length > 0)
		{
			// Send OSC message
			string address = "/project/param/1/value";// + variableName;
			OSCMessage message = new OSCMessage(address);
			message.AddValue(OSCValue.Float(variableValue));
			transmitter.Send(message);

			// Change local VFX value
			equationVFX.SetFloat(variableName, variableValue);

			// Update dashboard values (order by last update time)
			Array.Sort<PlayVariable>(sortedVariables);

			UpdateDashboardText();
		}

		// Handle update if one dashboard mode toggles changed
		if (lastDashboardFadingToggle != dashboardFadingToggle || lastDashboardOrderToggle != dashboardOrderToggle)
		{
			lastDashboardFadingToggle = dashboardFadingToggle;
			lastDashboardOrderToggle = dashboardOrderToggle;

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
		// Update black overlay opacity
		blackOverlayImg.color = new Color(1, 1, 1, blackOverlayOpacity);

		// Update VFX axes opacity
		equationVFX.SetFloat("axesOpacity", axesOpacity);
		if (axesOpacity == 0)
		{
			equationVFX.SetBool("toggleAxes", false);
		}
		else
		{
			equationVFX.SetBool("toggleAxes", true);
		}
		
		// Update coordinates opacity
		foreach (TextMeshProUGUI _coord in _coordinates)
		{
			_coord.color = new Color(1, 1, 1, coordinatesOpacity);
		}

		// Update equation image opacity
		equationImg.color = new Color(1, 1, 1, equationOpacity);
		
		// Update epilepsy warning opacity
		epilepticSlot1.color = new Color(1, 1, 1, epilepsyOpacity);
		epilepticSlot2.color = new Color(1, 1, 1, epilepsyOpacity);

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
		cam.revolutionSpeed = camSpeed;
		cam.baseRotAngleY = camBaseAngle;
		cam.radius = camRadius;

		// Update value buffers
		lastCamSpeed = camSpeed;
		lastCamBaseAngle = camBaseAngle;
		lastCamRadius = camRadius;
	}

	void UpdateDashboardText()
	{
		// Update text content
		if (dashboardOrderToggle)
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
			slot10.text = sortedVariables[9].getText();
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
			slot10.text = playVariables[9].getText();
		}

	}

	void UpdateDashboardOpacities()
	{
		// Full color (white) or slowly fading? Raw (unordered) or reordered?
		if (dashboardFadingToggle && dashboardOrderToggle)
		{
			slot1.color = new Color(1, 1, 1, sortedVariables[0].getOpacity() * dashboardOpacity);
			slot2.color = new Color(1, 1, 1, sortedVariables[1].getOpacity() * dashboardOpacity);
			slot3.color = new Color(1, 1, 1, sortedVariables[2].getOpacity() * dashboardOpacity);
			slot4.color = new Color(1, 1, 1, sortedVariables[3].getOpacity() * dashboardOpacity);
			slot5.color = new Color(1, 1, 1, sortedVariables[4].getOpacity() * dashboardOpacity);
			slot6.color = new Color(1, 1, 1, sortedVariables[5].getOpacity() * dashboardOpacity);
			slot7.color = new Color(1, 1, 1, sortedVariables[6].getOpacity() * dashboardOpacity);
			slot8.color = new Color(1, 1, 1, sortedVariables[7].getOpacity() * dashboardOpacity);
			slot9.color = new Color(1, 1, 1, sortedVariables[8].getOpacity() * dashboardOpacity);
			slot10.color = new Color(1, 1, 1, sortedVariables[9].getOpacity() * dashboardOpacity);
		}
		else if (dashboardFadingToggle && !dashboardOrderToggle)
		{
			slot1.color = new Color(1, 1, 1, playVariables[0].getOpacity() * dashboardOpacity);
			slot2.color = new Color(1, 1, 1, playVariables[1].getOpacity() * dashboardOpacity);
			slot3.color = new Color(1, 1, 1, playVariables[2].getOpacity() * dashboardOpacity);
			slot4.color = new Color(1, 1, 1, playVariables[3].getOpacity() * dashboardOpacity);
			slot5.color = new Color(1, 1, 1, playVariables[4].getOpacity() * dashboardOpacity);
			slot6.color = new Color(1, 1, 1, playVariables[5].getOpacity() * dashboardOpacity);
			slot7.color = new Color(1, 1, 1, playVariables[6].getOpacity() * dashboardOpacity);
			slot8.color = new Color(1, 1, 1, playVariables[7].getOpacity() * dashboardOpacity);
			slot9.color = new Color(1, 1, 1, playVariables[8].getOpacity() * dashboardOpacity);
			slot10.color = new Color(1, 1, 1, playVariables[9].getOpacity() * dashboardOpacity);
		}
		else
		{
			slot1.color = new Color(1, 1, 1, 1 * dashboardOpacity);
			slot2.color = new Color(1, 1, 1, 1 * dashboardOpacity);
			slot3.color = new Color(1, 1, 1, 1 * dashboardOpacity);
			slot4.color = new Color(1, 1, 1, 1 * dashboardOpacity);
			slot5.color = new Color(1, 1, 1, 1 * dashboardOpacity);
			slot6.color = new Color(1, 1, 1, 1 * dashboardOpacity);
			slot7.color = new Color(1, 1, 1, 1 * dashboardOpacity);
			slot8.color = new Color(1, 1, 1, 1 * dashboardOpacity);
			slot9.color = new Color(1, 1, 1, 1 * dashboardOpacity);
			slot10.color = new Color(1, 1, 1, 1 * dashboardOpacity);
		}

	}
}
