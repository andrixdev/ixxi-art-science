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
	public TextMeshProUGUI slot1bis; 
	public TextMeshProUGUI slot2;
	public TextMeshProUGUI slot2bis;
	public TextMeshProUGUI slot3;
	public TextMeshProUGUI slot3bis;
	public TextMeshProUGUI slot4;
	public TextMeshProUGUI slot4bis;
	public TextMeshProUGUI slot5;
	public TextMeshProUGUI slot5bis;
	public TextMeshProUGUI slot6;
	public TextMeshProUGUI slot6bis;
	public TextMeshProUGUI slot7;
	public TextMeshProUGUI slot7bis;
	public TextMeshProUGUI slot8;
	public TextMeshProUGUI slot8bis;
	public TextMeshProUGUI slot9;
	public TextMeshProUGUI slot9bis;

	// Axes coordinates canvas
	public Canvas _coordinatesCanvas;
	private TextMeshProUGUI[] _coordinates;

	// Sphere overlay material
	public Renderer sphereBlackOverlayMeshRenderer;
	private Material sphereBlackOverlayMaterial;

	// Equation image
	//public Image equationImg;
	
	// Panels
	public CanvasGroup panel0;
	public CanvasGroup panel1;
	public CanvasGroup panel2;
	public CanvasGroup panel3;
	public CanvasGroup panel4;
	public CanvasGroup panel5;
	public CanvasGroup panel6;

	// Panel 3 vfxs
	public VisualEffect panel3vfx1;
	public VisualEffect panel3vfx2;
	public VisualEffect panel3vfx3;

	// Rotation
	public RotGently rot;

	// Controls : Master system variables
	[Header("Master system variables")]
	
	[Range(0.0f, 1.0f)]
	public float masterIntensity = 1.0f;
	private float lastMasterIntensity = 1.0f;
	
	[Range(0.0f, 1.0f)]
	public float equationIntensity = 1.0f;
	private float lastEquationIntensity = 1.0f;
	
	[Range(0.0f, 1.0f)]
	public float turbulenceIntensity = 0.0f;
	private float lastTurbulenceIntensity = 1.0f;

	// Controls : Equation variables
	[Header("Equation variables")]

	[Range(0.0f, 5.0f)]
	public float k0 = 0.0f;
	public PlayVariable K0;

	[Range(0.0f, 5.0f)]
	public float e0 = 0.0f;
	public PlayVariable E0;
	
	[Range(0.0f, 2.0f)]
	public float lambda = 1.5f;
	public PlayVariable Lambda;

	[Range(0.0f, 2.0f)]
	public float eta = 0.0f;
	public PlayVariable Eta;
	
	[Range(0.0f, 1.0f)]
	public float nu = 0.1f;
	public PlayVariable Nu;
	
	[Range(-1.0f, 3.0f)]
	public float a = 1.0f;
	public PlayVariable A;
	
	[Range(0.0f, 3.0f)]
	public float b = 0.5f;
	public PlayVariable B;
	
	[Range(0.0f, 2.0f)]
	public float alpha = 0.25f;
	public PlayVariable Alpha;
	
	[Range(0.0f, 3.0f)]
	public float beta = 1.25f;
	public PlayVariable Beta;
	
	private PlayVariable[] playVariables;
	private PlayVariable[] sortedVariables;

	// Controls : Visual controls
	[Header("Visual controls")]

	[Range(0.0f, 1.0f)]
	public float blackOverlayOpacity = 0.0f;
	private float lastBlackOverlayOpacity = 0.0f;

	[Range(0.0f, 1.0f)]
	public float panel0Opacity = 0.0f;
	private float lastPanel0Opacity = 0.0f;

	[Range(0.0f, 1.0f)]
	public float panel1Opacity = 0.0f;
	private float lastPanel1Opacity = 0.0f;

	[Range(0.0f, 1.0f)]
	public float panel2Opacity = 0.0f;
	private float lastPanel2Opacity = 0.0f;
	
	[Range(0.0f, 1.0f)]
	public float panel3Opacity = 0.0f;
	private float lastPanel3Opacity = 0.0f;
	
	[Range(0.0f, 1.0f)]
	public float panel4Opacity = 0.0f;
	private float lastPanel4Opacity = 0.0f;
	
	[Range(0.0f, 1.0f)]
	public float panel5Opacity = 0.0f;
	private float lastPanel5Opacity = 0.0f;
	
	[Range(0.0f, 1.0f)]
	public float panel6Opacity = 0.0f;
	private float lastPanel6Opacity = 0.0f;

	[Range(0.0f, 1.0f)]
	public float axesOpacity = 0.0f;
	private float lastAxesOpacity = 0.0f;

	[Range(0.0f, 1.0f)]
	public float coordinatesOpacity = 0.0f;
	private float lastCoordinatesOpacity = 0.0f;

	[Range(0.0f, 1.0f)]
	public float dashboardOpacity = 1.0f;
	public bool dashboardMode = true;
	private bool lastDashboardMode = true;

	public bool togglePositiveCorner = true;
	private bool lastTogglePositionCorner = true;

	[Range(-1.0f, 1.0f)]
	public float rotSpeed = 0.15f;
	private float lastRotSpeed = 0.1f;

	[Range(-400.0f, 400.0f)]
	public float rotBaseAngle = 0.0f;
	private float lastRotBaseAngle = 0.0f;

	[Range(0.0f, 1.0f)]
	public float motionLerpValue = 0.0f;
	private float lastMotionLerpValue = 0.0f;

	[Range(0.0f, 2.0f)]
	public float spawnShapeRotSpeed = 0.0f;
	private float lastSpawnShapeRotSpeed = 0.0f;

	protected void Start()
	{
		// Get all child coordinates from Coordinates Canvas
		_coordinates = _coordinatesCanvas.GetComponentsInChildren<TextMeshProUGUI>();

		// Store all play variables in an array
		K0 = new PlayVariable("k", k0); // 0 appended in UI
		E0 = new PlayVariable("E", e0); // 0 appended in UI
		Lambda = new PlayVariable("λ", lambda); // α β γ δ ε π Δ ν
		Eta = new PlayVariable("η", eta);
		Nu = new PlayVariable("ν", nu);
		A = new PlayVariable("a", a);
		B = new PlayVariable("b", b);
		Alpha = new PlayVariable("α", alpha);
		Beta = new PlayVariable("β", beta);

		playVariables = new PlayVariable[9];
		playVariables[0] = K0;
		playVariables[1] = E0;
		playVariables[2] = Lambda;
		playVariables[3] = Eta;
		playVariables[4] = Nu;
		playVariables[5] = A;
		playVariables[6] = B;
		playVariables[7] = Alpha;
		playVariables[8] = Beta;

		// Prepare array that will order displayed value based on last update time
		sortedVariables = new PlayVariable[9];
		playVariables.CopyTo(sortedVariables, 0);

		// Prepare OSC message reception
		receiver.Bind("/k0", HandleMessage);
		receiver.Bind("/e0", HandleMessage);
		receiver.Bind("/lambda", HandleMessage);
		receiver.Bind("/eta", HandleMessage);
		receiver.Bind("/nu", HandleMessage);
		receiver.Bind("/a", HandleMessage);
		receiver.Bind("/b", HandleMessage);
		receiver.Bind("/alpha", HandleMessage);
		receiver.Bind("/beta", HandleMessage);

		receiver.Bind("/overlay-opacity", HandleMessage);
		receiver.Bind("/panel-0-opacity", HandleMessage);
		receiver.Bind("/panel-1-opacity", HandleMessage);
		receiver.Bind("/panel-2-opacity", HandleMessage);
		receiver.Bind("/panel-3-opacity", HandleMessage);
		receiver.Bind("/panel-4-opacity", HandleMessage);
		receiver.Bind("/panel-5-opacity", HandleMessage);
		receiver.Bind("/panel-6-opacity", HandleMessage);
		receiver.Bind("/axes-opacity", HandleMessage);
		receiver.Bind("/coordinates-opacity", HandleMessage);
		receiver.Bind("/dashboard-opacity", HandleMessage);
		receiver.Bind("/rot-speed", HandleMessage);
		receiver.Bind("/rot-base-angle", HandleMessage);
		receiver.Bind("/motion-lerp", HandleMessage);
		receiver.Bind("/spawn-shape-rot-speed", HandleMessage);

		receiver.Bind("/dashboard-mode", HandleBoolMessage);
		receiver.Bind("/toggle-corner", HandleBoolMessage);

		receiver.Bind("/master", HandleMessage);
		receiver.Bind("/equation", HandleMessage);
		receiver.Bind("/turbulence", HandleMessage);

		// Prepare material of black overlay sphere
		sphereBlackOverlayMaterial = sphereBlackOverlayMeshRenderer.material;

		// Boop the update functions to initialize the values
		UpdateOpacities();
		UpdateRotation();
		UpdateDashboardText();
		UpdateDashboardOpacities();
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

		if (address == "/k0") {	k0 = 5 * value; } // [0, 5]
		else if (address == "/e0") { e0 = 5 * value; } // [0, 5]
		else if (address == "/lambda") { lambda = 2 * value; } // [0, 2]
		else if (address == "/eta") { eta = 2 * value; } // [0, 2]
		else if (address == "/nu") { nu = 2 * value; } // [0, 2]
		else if (address == "/a") { a = -1 + 4 * value; } // [-1, 3]
		else if (address == "/b") { b = 3 * value; } // [0, 3]
		else if (address == "/alpha") { alpha = 2 * value; } // [0, 2]
		else if (address == "/beta") { beta = 3 * value; } // [0, 3]
		else if (address == "/overlay-opacity") { blackOverlayOpacity = value; }
		else if (address == "/panel-0-opacity") { panel0Opacity = value; }
		else if (address == "/panel-1-opacity") { panel1Opacity = value; }
		else if (address == "/panel-2-opacity") { panel2Opacity = value; }
		else if (address == "/panel-3-opacity") { panel3Opacity = value; }
		else if (address == "/panel-4-opacity") { panel4Opacity = value; }
		else if (address == "/panel-5-opacity") { panel5Opacity = value; }
		else if (address == "/panel-6-opacity") { panel6Opacity = value; }
		else if (address == "/axes-opacity") { axesOpacity = value; }
		else if (address == "/coordinates-opacity") { coordinatesOpacity = value; }
		else if (address == "/dashboard-opacity") { dashboardOpacity = value; }
		else if (address == "/rot-speed") { rotSpeed = -1 + 2 * value; } // [-1, 1]
		else if (address == "/rot-base-angle") { rotBaseAngle = -400 + 800 * value; } // [-400, 400]
		else if (address == "/motion-lerp") { motionLerpValue = value; }
		else if (address == "/spawn-shape-rot-speed") { spawnShapeRotSpeed = value; }
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

		if (address == "/dashboard-mode") {	dashboardMode = value; }
		else if (address == "/toggle-corner") { togglePositiveCorner = value; }
		else
		{
			Debug.LogWarning("OSC address not recognized (Bool section): " + address);
		}
	}

	void Check()
	{

		// Detect change for equation variables
		bool hasChanged = false;
		if (k0 != playVariables[0].value) 		   			    { playVariables[0].update(k0); equationVFX.SetFloat("k0", playVariables[0].value); hasChanged = true; }
		if (e0 != playVariables[1].value)						{ playVariables[1].update(e0); equationVFX.SetFloat("E0", playVariables[1].value); hasChanged = true; }
		if (lambda != playVariables[2].value) 					{ playVariables[2].update(lambda); equationVFX.SetFloat("lambda", playVariables[2].value); hasChanged = true; }
		if (eta != playVariables[3].value) 				 		{ playVariables[3].update(eta); equationVFX.SetFloat("eta", playVariables[3].value); hasChanged = true; }
		if (nu != playVariables[4].value) 		 				{ playVariables[4].update(nu); equationVFX.SetFloat("nu", playVariables[4].value); hasChanged = true; }
		if (a != playVariables[5].value) 					 	{ playVariables[5].update(a); equationVFX.SetFloat("a", playVariables[5].value); hasChanged = true; }
		if (b != playVariables[6].value) 						{ playVariables[6].update(b); equationVFX.SetFloat("b", playVariables[6].value); hasChanged = true; }
		if (alpha != playVariables[7].value) 	 				{ playVariables[7].update(alpha); equationVFX.SetFloat("alpha", playVariables[7].value); hasChanged = true; }
		if (beta != playVariables[8].value) 	 				{ playVariables[8].update(beta); equationVFX.SetFloat("beta", playVariables[8].value); hasChanged = true; }
		if (hasChanged)
		{
			// Update dashboard values (order by last update time)
			Array.Sort<PlayVariable>(sortedVariables);

			UpdateDashboardText();
		}

		// Detect change & smooth with a square law for master inputs
		if (masterIntensity != lastMasterIntensity) 			{ equationVFX.SetFloat("masterIntensity", Mathf.Pow(masterIntensity, 2.0f)); }
		if (equationIntensity != lastEquationIntensity) 		{ equationVFX.SetFloat("equationIntensity", Mathf.Pow(equationIntensity, 2.0f)); }
		if (turbulenceIntensity != lastTurbulenceIntensity) 	{ equationVFX.SetFloat("turbulenceIntensity", Mathf.Pow(turbulenceIntensity, 2.0f)); }

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
		if (
			lastBlackOverlayOpacity != blackOverlayOpacity
			|| lastAxesOpacity != axesOpacity
			|| lastCoordinatesOpacity != coordinatesOpacity
			|| lastPanel0Opacity != panel0Opacity
			|| lastPanel1Opacity != panel1Opacity
			|| lastPanel2Opacity != panel2Opacity
			|| lastPanel3Opacity != panel3Opacity
			|| lastPanel4Opacity != panel4Opacity
			|| lastPanel5Opacity != panel5Opacity
			|| lastPanel6Opacity != panel6Opacity
		)
		{
			UpdateOpacities();
		}
		
		// Update rotation variables if one value changed
		if (lastRotSpeed != rotSpeed ||	lastRotBaseAngle != rotBaseAngle ||	lastMotionLerpValue != motionLerpValue || lastSpawnShapeRotSpeed != spawnShapeRotSpeed)
		{
			UpdateRotation();
		}

		// Always update dashboard text opacities
		UpdateDashboardOpacities();

	}

	void UpdateOpacities()
	{
		// Update black overlay opacity (smoothed with ~cubic root power law)
		sphereBlackOverlayMaterial.color = new Color(0, 0, 0, Mathf.Pow(blackOverlayOpacity, 0.3f));

		// Update VFX axes opacity (smoothed with square power law)
		equationVFX.SetFloat("axesOpacity", Mathf.Pow(axesOpacity, 2.0f));
		
		// Update coordinates opacity (smoothed with square power law)
		foreach (TextMeshProUGUI _coord in _coordinates)
		{
			_coord.color = new Color(1, 1, 1, Mathf.Pow(coordinatesOpacity, 2.0f));
		}

		// Keep commented: updating image alpha
		//equationImg.color = new Color(1, 1, 1, Mathf.Pow(equationOpacity, 2.0f));
		// Keep commented: updating text alpha
		//epilepticSlot1.color = new Color(1, 1, 1, Mathf.Pow(epilepsyOpacity, 2.0f));

		// Update panels opacities (smoothed with square power law)
		panel0.alpha = Mathf.Clamp01(Mathf.Pow(panel0Opacity, 2.0f));
		panel1.alpha = Mathf.Clamp01(Mathf.Pow(panel1Opacity, 2.0f));
		panel2.alpha = Mathf.Clamp01(Mathf.Pow(panel2Opacity, 2.0f));
		panel3.alpha = Mathf.Clamp01(Mathf.Pow(panel3Opacity, 2.0f));
		panel4.alpha = Mathf.Clamp01(Mathf.Pow(panel4Opacity, 2.0f));
		panel5.alpha = Mathf.Clamp01(Mathf.Pow(panel5Opacity, 2.0f));
		panel6.alpha = Mathf.Clamp01(Mathf.Pow(panel6Opacity, 2.0f));

		// Special treat for VFXs inside panels (not affected by CanvasGroup alpha)
		panel3vfx1.SetFloat("alphaMult", Mathf.Pow(panel3Opacity, 2.0f));
		panel3vfx2.SetFloat("alphaMult", Mathf.Pow(panel3Opacity, 2.0f));
		panel3vfx3.SetFloat("alphaMult", Mathf.Pow(panel3Opacity, 2.0f));

		// Update value buffers
		lastBlackOverlayOpacity = blackOverlayOpacity;
		lastAxesOpacity = axesOpacity;
		lastCoordinatesOpacity = coordinatesOpacity;
		lastPanel0Opacity = panel0Opacity;
		lastPanel1Opacity = panel1Opacity;
		lastPanel2Opacity = panel2Opacity;
		lastPanel3Opacity = panel3Opacity;
		lastPanel4Opacity = panel4Opacity;
		lastPanel5Opacity = panel5Opacity;
		lastPanel6Opacity = panel6Opacity;

	}

	void UpdateRotation()
	{
		// Update rotation variables
		float smoothedRotSpeed = Mathf.Sign(rotSpeed) * Mathf.Pow(Mathf.Abs(rotSpeed), 3.0f);
		rot.revolutionSpeed = smoothedRotSpeed;
		rot.baseRotAngleY = rotBaseAngle;
		rot.motionLerpValue = motionLerpValue;

		// Update value buffers
		lastRotSpeed = rotSpeed;
		lastRotBaseAngle = rotBaseAngle;
		lastMotionLerpValue = motionLerpValue;
		lastSpawnShapeRotSpeed = spawnShapeRotSpeed;
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
		PlayVariable[] sv = sortedVariables;

		// dashboardMode : true => fading with time and reordered (last changed variable on top), false => always displayed and in the same order
		if (dashboardMode)
		{
			slot1.color = new Color(1, 1, 1, sv[0].getOpacity() * dashOpa);
			float bis1opa = (sv[0].name == "k" || sv[0].name == "E") ? sv[0].getOpacity() * dashOpa : 0;
			slot1bis.color = new Color(1, 1, 1, bis1opa);
			
			slot2.color = new Color(1, 1, 1, sv[1].getOpacity() * dashOpa);
			float bis2opa = (sv[1].name == "k" || sv[1].name == "E") ? sv[1].getOpacity() * dashOpa : 0;
			slot2bis.color = new Color(1, 1, 1, bis2opa);
			
			slot3.color = new Color(1, 1, 1, sv[2].getOpacity() * dashOpa);
			float bis3opa = (sv[2].name == "k" || sv[2].name == "E") ? sv[2].getOpacity() * dashOpa : 0;
			slot3bis.color = new Color(1, 1, 1, bis3opa);
			
			slot4.color = new Color(1, 1, 1, sv[3].getOpacity() * dashOpa);
			float bis4opa = (sv[3].name == "k" || sv[3].name == "E") ? sv[3].getOpacity() * dashOpa : 0;
			slot4bis.color = new Color(1, 1, 1, bis4opa);
			
			slot5.color = new Color(1, 1, 1, sv[4].getOpacity() * dashOpa);
			float bis5opa = (sv[4].name == "k" || sv[4].name == "E") ? sv[4].getOpacity() * dashOpa : 0;
			slot5bis.color = new Color(1, 1, 1, bis5opa);
			
			slot6.color = new Color(1, 1, 1, sv[5].getOpacity() * dashOpa);
			float bis6opa = (sv[5].name == "k" || sv[5].name == "E") ? sv[5].getOpacity() * dashOpa : 0;
			slot6bis.color = new Color(1, 1, 1, bis6opa);
			
			slot7.color = new Color(1, 1, 1, sv[6].getOpacity() * dashOpa);
			float bis7opa = (sv[6].name == "k" || sv[6].name == "E") ? sv[6].getOpacity() * dashOpa : 0;
			slot7bis.color = new Color(1, 1, 1, bis7opa);
			
			slot8.color = new Color(1, 1, 1, sv[7].getOpacity() * dashOpa);
			float bis8opa = (sv[7].name == "k" || sv[7].name == "E") ? sv[7].getOpacity() * dashOpa : 0;
			slot8bis.color = new Color(1, 1, 1, bis8opa);
			
			slot9.color = new Color(1, 1, 1, sv[8].getOpacity() * dashOpa);
			float bis9opa = (sv[8].name == "k" || sv[8].name == "E") ? sv[8].getOpacity() * dashOpa : 0;
			slot9bis.color = new Color(1, 1, 1, bis9opa);
		}
		else
		{
			slot1.color = new Color(1, 1, 1, 1 * dashOpa);
			slot1bis.color = new Color(1, 1, 1, 1 * dashOpa);
			slot2.color = new Color(1, 1, 1, 1 * dashOpa);
			slot2bis.color = new Color(1, 1, 1, 1 * dashOpa);
			slot3.color = new Color(1, 1, 1, 1 * dashOpa);
			slot3bis.color = new Color(1, 1, 1, 0);
			slot4.color = new Color(1, 1, 1, 1 * dashOpa);
			slot4bis.color = new Color(1, 1, 1, 0);
			slot5.color = new Color(1, 1, 1, 1 * dashOpa);
			slot5bis.color = new Color(1, 1, 1, 0);
			slot6.color = new Color(1, 1, 1, 1 * dashOpa);
			slot6bis.color = new Color(1, 1, 1, 0);
			slot7.color = new Color(1, 1, 1, 1 * dashOpa);
			slot7bis.color = new Color(1, 1, 1, 0);
			slot8.color = new Color(1, 1, 1, 1 * dashOpa);
			slot8bis.color = new Color(1, 1, 1, 0);
			slot9.color = new Color(1, 1, 1, 1 * dashOpa);
			slot9bis.color = new Color(1, 1, 1, 0);
		}

	}
}
