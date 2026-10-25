/**
 * ANDRIX © 2026
 * Backup control panel: press Space to show / hide on-screen sliders for every
 * performance variable of EquationOSCAndVFXController, in case OSC control fails.
 * The panel writes directly into the controller fields (exactly like OSC does),
 * OSC keeps working while it is open, and the sliders follow incoming values live.
 * The UI is built at runtime, no scene setup required (auto-attached on scene load).
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BackupControlPanel : MonoBehaviour
{
	[Header("Target")]
	public EquationOSCAndVFXController controller;

	[Header("Behaviour")]
	public KeyCode toggleKey = KeyCode.Space;
	public KeyCode nextDisplayKey = KeyCode.RightArrow;
	public KeyCode previousDisplayKey = KeyCode.LeftArrow;
	[Tooltip("0 = Display 1. Arrow keys cycle Display 1 > 2 > 3, skipping displays that are not connected in a build.")]
	public int targetDisplay = 0;
	public bool startVisible = false;
	public float fadeDuration = 0.15f;

	[Header("Fonts (found in loaded assets if left empty)")]
	public TMP_FontAsset uiFont;
	public TMP_FontAsset symbolFont;

	// Palette
	private static readonly Color BackdropColor = new Color(0f, 0f, 0f, 0.3f);
	private static readonly Color PanelColor = new Color(0f, 0f, 0f, 0.9f);
	private static readonly Color LineColor = new Color(1f, 1f, 1f, 0.12f);
	private static readonly Color TrackColor = new Color(1f, 1f, 1f, 0.16f);
	private static readonly Color FillColor = new Color(1f, 1f, 1f, 0.85f);
	private static readonly Color TextColor = new Color(1f, 1f, 1f, 0.92f);
	private static readonly Color DimTextColor = new Color(1f, 1f, 1f, 0.45f);

	// Layout (reference resolution 1920x1080)
	private const float PanelWidth = 1560f;
	private const float PanelHeight = 800f;
	private const float Padding = 40f;
	private const float ColumnGap = 50f;
	private const float TitleHeight = 76f;
	private const float HeaderHeight = 44f;
	private const float RowHeight = 50f;
	private const float LabelWidth = 130f;
	private const float ValueWidth = 72f;

	// Displays the panel can be moved to with the arrow keys (Display 1 to 3)
	private const int CycledDisplayCount = 3;

	private Canvas canvas;
	private CanvasGroup canvasGroup;
	private TextMeshProUGUI hint;
	private bool visible;
	private Sprite circleSprite;
	private readonly List<Control> controls = new List<Control>();
	private Transform currentColumn;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Bootstrap()
	{
		if (FindAnyObjectByType<BackupControlPanel>() != null) return;
		EquationOSCAndVFXController target = FindAnyObjectByType<EquationOSCAndVFXController>();
		if (target != null) target.gameObject.AddComponent<BackupControlPanel>();
	}

	void Start()
	{
		if (controller == null) controller = FindAnyObjectByType<EquationOSCAndVFXController>();
		if (controller == null)
		{
			Debug.LogWarning("BackupControlPanel: no EquationOSCAndVFXController found, panel disabled.");
			enabled = false;
			return;
		}

		if (uiFont == null) uiFont = FindFont("Outfit-Light SDF", "Outfit-Regular SDF", "Outfit-Thin SDF");
		if (symbolFont == null) symbolFont = FindFont("EBGaramond-Regular SDF greek and latin");

		circleSprite = CreateCircleSprite(64);
		EnsureEventSystem();
		BuildUI();

		visible = startVisible;
		canvasGroup.alpha = visible ? 1f : 0f;
		canvasGroup.blocksRaycasts = visible;
		canvas.enabled = visible;
	}

	void Update()
	{
		if (Input.GetKeyDown(toggleKey))
		{
			visible = !visible;
			canvasGroup.blocksRaycasts = visible;
			if (visible) canvas.enabled = true;
		}
		if (Input.GetKeyDown(nextDisplayKey)) MoveToDisplay(1);
		else if (Input.GetKeyDown(previousDisplayKey)) MoveToDisplay(-1);

		// Fade in / out
		float targetAlpha = visible ? 1f : 0f;
		if (canvasGroup.alpha != targetAlpha)
		{
			float step = fadeDuration > 0f ? Time.unscaledDeltaTime / fadeDuration : 1f;
			canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, step);
			if (canvasGroup.alpha == 0f) canvas.enabled = false;
		}

		if (!canvas.enabled) return;

		// Never keep a panel control selected, so Space / arrows / Enter don't act on it
		EventSystem es = EventSystem.current;
		if (es != null && es.currentSelectedGameObject != null && es.currentSelectedGameObject.transform.IsChildOf(canvas.transform))
		{
			es.SetSelectedGameObject(null);
		}

		// Follow values coming from OSC (or the inspector)
		foreach (Control control in controls) control.Sync();
	}

	void OnDestroy()
	{
		if (canvas != null) Destroy(canvas.gameObject);
	}

	// ---------------------------------------------------------------- Displays

	// Right: 1 > 2 > 3 > 1, left: 3 > 2 > 1 > 3 (unavailable displays are skipped)
	void MoveToDisplay(int direction)
	{
		for (int step = 1; step < CycledDisplayCount; step++)
		{
			int candidate = ((targetDisplay + direction * step) % CycledDisplayCount + CycledDisplayCount) % CycledDisplayCount;
			if (IsDisplayAvailable(candidate))
			{
				targetDisplay = candidate;
				canvas.targetDisplay = targetDisplay;
				UpdateHint();
				return;
			}
		}
	}

	static bool IsDisplayAvailable(int index)
	{
		// In the editor, any Game view can show any display
		if (Application.isEditor) return true;
		return index < Display.displays.Length && (index == 0 || Display.displays[index].active);
	}

	void UpdateHint()
	{
		hint.text = "DISPLAY " + (targetDisplay + 1) + "  ·  ARROWS to move  ·  SPACE to hide  ·  OSC still live";
	}

	// ---------------------------------------------------------------- Layout

	void BuildUI()
	{
		EquationOSCAndVFXController c = controller;

		GameObject canvasGO = new GameObject("Backup Control Panel (Canvas)");
		canvas = canvasGO.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.sortingOrder = 1000;
		canvas.targetDisplay = targetDisplay;
		CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920f, 1080f);
		scaler.matchWidthOrHeight = 0.5f;
		canvasGO.AddComponent<GraphicRaycaster>();
		canvasGroup = canvasGO.AddComponent<CanvasGroup>();

		// Backdrop (also swallows clicks outside the panel)
		Image backdrop = NewImage("Backdrop", canvasGO.transform, BackdropColor);
		Stretch(backdrop.rectTransform);

		// Panel with thin frame
		Image panel = NewImage("Panel", canvasGO.transform, PanelColor);
		RectTransform panelRT = panel.rectTransform;
		panelRT.sizeDelta = new Vector2(PanelWidth, PanelHeight);
		AddFrame(panelRT);

		// Title bar
		TextMeshProUGUI title = NewText("Title", panelRT, "BACKUP CONTROLS", uiFont, 20f, TextColor, TextAlignmentOptions.MidlineLeft);
		title.characterSpacing = 20f;
		Place(title.rectTransform, new Vector2(0f, 1f), new Vector2(0.5f, 1f), new Vector2(Padding, -TitleHeight), new Vector2(0f, 0f));

		hint = NewText("Hint", panelRT, "", uiFont, 14f, DimTextColor, TextAlignmentOptions.MidlineRight);
		UpdateHint();
		hint.characterSpacing = 6f;
		Place(hint.rectTransform, new Vector2(0.5f, 1f), new Vector2(1f, 1f), new Vector2(0f, -TitleHeight), new Vector2(-Padding, 0f));

		Image titleLine = NewImage("Title Line", panelRT, LineColor);
		Place(titleLine.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(Padding, -TitleHeight - 1f), new Vector2(-Padding, -TitleHeight));

		// Column 1 : equation variables (same mapping as OSC)
		BeginColumn(panelRT, 0);
		Header("Equation", true);
		FloatRow("k<sub>0</sub>", "k0", 0f, 5f, () => c.k0, v => c.k0 = v);
		FloatRow("E<sub>0</sub>", "E0", 0f, 5f, () => c.e0, v => c.e0 = v);
		FloatRow("λ", "lambda", 0.1f, 2f, () => c.lambda, v => c.lambda = v);
		FloatRow("η", "eta", 0f, 2f, () => c.eta, v => c.eta = v);
		FloatRow("ν", "nu", 0f, 2f, () => c.nu, v => c.nu = v);
		FloatRow("a", "", -1f, 3f, () => c.a, v => c.a = v);
		FloatRow("b", "", 0f, 3f, () => c.b, v => c.b = v);
		FloatRow("α", "alpha", 0f, 2f, () => c.alpha, v => c.alpha = v);
		FloatRow("β", "beta", 0f, 3f, () => c.beta, v => c.beta = v);

		// Column 2 : master, motion, modes
		BeginColumn(panelRT, 1);
		Header("Master", true);
		FloatRow(null, "Master", 0f, 1f, () => c.masterIntensity, v => c.masterIntensity = v);
		FloatRow(null, "Equation", 0f, 1f, () => c.equationIntensity, v => c.equationIntensity = v);
		FloatRow(null, "Turbulence", 0f, 1f, () => c.turbulenceIntensity, v => c.turbulenceIntensity = v);
		Header("Motion", false);
		FloatRow(null, "Rot speed", -1f, 1f, () => c.rotSpeed, v => c.rotSpeed = v, "+0.00;-0.00;0.00");
		FloatRow(null, "Rot angle", -400f, 400f, () => c.rotBaseAngle, v => c.rotBaseAngle = v, "0");
		FloatRow(null, "Motion lerp", 0f, 1f, () => c.motionLerpValue, v => c.motionLerpValue = v);
		FloatRow(null, "Spawn rot", 0f, 4f, () => c.spawnShapeRotSpeed, v => c.spawnShapeRotSpeed = v);
		Header("Modes", false);
		BoolRow("Language", "EN", "FR", () => c.languageIsEnglish, v => c.languageIsEnglish = v);
		BoolRow("Dashboard", "FADE", "FIXED", () => c.dashboardMode, v => c.dashboardMode = v);
		BoolRow("Positive corner", "ON", "OFF", () => c.togglePositiveCorner, v => c.togglePositiveCorner = v);

		// Column 3 : layers opacities
		BeginColumn(panelRT, 2);
		Header("Layers", true);
		FloatRow(null, "Black overlay", 0f, 1f, () => c.blackOverlayOpacity, v => c.blackOverlayOpacity = v);
		FloatRow(null, "Panel 0", 0f, 1f, () => c.panel0Opacity, v => c.panel0Opacity = v);
		FloatRow(null, "Panel 1", 0f, 1f, () => c.panel1Opacity, v => c.panel1Opacity = v);
		FloatRow(null, "Panel 2", 0f, 1f, () => c.panel2Opacity, v => c.panel2Opacity = v);
		FloatRow(null, "Panel 3", 0f, 1f, () => c.panel3Opacity, v => c.panel3Opacity = v);
		FloatRow(null, "Panel 4", 0f, 1f, () => c.panel4Opacity, v => c.panel4Opacity = v);
		FloatRow(null, "Panel 5", 0f, 1f, () => c.panel5Opacity, v => c.panel5Opacity = v);
		FloatRow(null, "Panel 6", 0f, 1f, () => c.panel6Opacity, v => c.panel6Opacity = v);
		FloatRow(null, "Axes", 0f, 1f, () => c.axesOpacity, v => c.axesOpacity = v);
		FloatRow(null, "Coordinates", 0f, 1f, () => c.coordinatesOpacity, v => c.coordinatesOpacity = v);
		FloatRow(null, "Dashboard", 0f, 1f, () => c.dashboardOpacity, v => c.dashboardOpacity = v);
	}

	void BeginColumn(RectTransform panel, int index)
	{
		float columnWidth = (PanelWidth - 2f * Padding - 2f * ColumnGap) / 3f;
		float x = Padding + index * (columnWidth + ColumnGap);

		RectTransform column = NewRect("Column " + index, panel);
		column.anchorMin = new Vector2(0f, 1f);
		column.anchorMax = new Vector2(0f, 1f);
		column.pivot = new Vector2(0f, 1f);
		column.anchoredPosition = new Vector2(x, -TitleHeight - 12f);
		column.sizeDelta = new Vector2(columnWidth, PanelHeight - TitleHeight - 12f - Padding);

		VerticalLayoutGroup layout = column.gameObject.AddComponent<VerticalLayoutGroup>();
		layout.childControlWidth = true;
		layout.childControlHeight = true;
		layout.childForceExpandWidth = true;
		layout.childForceExpandHeight = false;

		currentColumn = column;
	}

	void Header(string text, bool first)
	{
		RectTransform header = NewRow("Header " + text, first ? HeaderHeight : HeaderHeight + 16f);

		TextMeshProUGUI label = NewText("Label", header, text.ToUpperInvariant(), uiFont, 13f, DimTextColor, TextAlignmentOptions.BottomLeft);
		label.characterSpacing = 18f;
		Place(label.rectTransform, Vector2.zero, Vector2.one, new Vector2(0f, 10f), Vector2.zero);

		Image line = NewImage("Line", header, LineColor);
		Place(line.rectTransform, Vector2.zero, new Vector2(1f, 0f), Vector2.zero, new Vector2(0f, 1f));
	}

	void FloatRow(string symbol, string name, float min, float max, Func<float> get, Action<float> set, string format = "0.00")
	{
		RectTransform row = NewRow("Row " + (string.IsNullOrEmpty(name) ? symbol : name), RowHeight);
		AddRowLabel(row, symbol, name);

		// Slider: invisible hit area, thin track, fill and round handle
		RectTransform sliderRT = NewRect("Slider", row);
		Place(sliderRT, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(LabelWidth, -12f), new Vector2(-ValueWidth - 8f, 12f));
		Stretch(NewImage("Hit Area", sliderRT, Color.clear).rectTransform);

		Image track = NewImage("Track", sliderRT, TrackColor);
		track.raycastTarget = false;
		Place(track.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0f, -1f), new Vector2(0f, 1f));

		RectTransform fillArea = NewRect("Fill Area", sliderRT);
		Place(fillArea, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(7f, -1f), new Vector2(-7f, 1f));
		Image fill = NewImage("Fill", fillArea, FillColor);
		fill.raycastTarget = false;
		Place(fill.rectTransform, Vector2.zero, new Vector2(0f, 1f), new Vector2(-7f, 0f), Vector2.zero);

		RectTransform handleArea = NewRect("Handle Area", sliderRT);
		Place(handleArea, Vector2.zero, Vector2.one, new Vector2(7f, 5f), new Vector2(-7f, -5f));
		Image handle = NewImage("Handle", handleArea, Color.white);
		handle.sprite = circleSprite;
		handle.rectTransform.sizeDelta = new Vector2(14f, 0f); // height follows the 14px handle area

		Slider slider = sliderRT.gameObject.AddComponent<Slider>();
		slider.fillRect = fill.rectTransform;
		slider.handleRect = handle.rectTransform;
		slider.targetGraphic = handle;
		slider.direction = Slider.Direction.LeftToRight;
		slider.minValue = min;
		slider.maxValue = max;
		slider.navigation = new Navigation { mode = Navigation.Mode.None };
		ColorBlock colors = slider.colors;
		colors.normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
		colors.highlightedColor = Color.white;
		colors.pressedColor = Color.white;
		colors.selectedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
		colors.fadeDuration = 0.08f;
		slider.colors = colors;
		slider.SetValueWithoutNotify(get());
		slider.onValueChanged.AddListener(v => set(v));

		PressTracker press = sliderRT.gameObject.AddComponent<PressTracker>();

		TextMeshProUGUI valueText = NewText("Value", row, "", uiFont, 16f, TextColor, TextAlignmentOptions.MidlineRight);
		Place(valueText.rectTransform, new Vector2(1f, 0f), Vector2.one, new Vector2(-ValueWidth, 0f), Vector2.zero);

		FloatControl control = new FloatControl(slider, press, valueText, get, format);
		control.Sync();
		controls.Add(control);
	}

	void BoolRow(string name, string onText, string offText, Func<bool> get, Action<bool> set)
	{
		RectTransform row = NewRow("Row " + name, RowHeight);
		AddRowLabel(row, null, name);

		// The whole row is clickable
		Image hit = NewImage("Hit Area", row, Color.clear);
		Stretch(hit.rectTransform);

		// Switch: pill track + round knob
		Image track = NewImage("Switch", row, TrackColor);
		track.raycastTarget = false;
		track.sprite = circleSprite;
		track.type = Image.Type.Sliced;
		track.pixelsPerUnitMultiplier = (circleSprite.border.x) / 10f;
		RectTransform trackRT = track.rectTransform;
		trackRT.anchorMin = trackRT.anchorMax = new Vector2(0f, 0.5f);
		trackRT.pivot = new Vector2(0f, 0.5f);
		trackRT.anchoredPosition = new Vector2(LabelWidth, 0f);
		trackRT.sizeDelta = new Vector2(40f, 20f);

		Image knob = NewImage("Knob", trackRT, Color.white);
		knob.raycastTarget = false;
		knob.sprite = circleSprite;
		RectTransform knobRT = knob.rectTransform;
		knobRT.anchorMin = knobRT.anchorMax = new Vector2(0f, 0.5f);
		knobRT.sizeDelta = new Vector2(14f, 14f);

		Toggle toggle = row.gameObject.AddComponent<Toggle>();
		toggle.targetGraphic = hit;
		toggle.transition = Selectable.Transition.None;
		toggle.navigation = new Navigation { mode = Navigation.Mode.None };
		toggle.SetIsOnWithoutNotify(get());
		toggle.onValueChanged.AddListener(v => set(v));

		TextMeshProUGUI valueText = NewText("Value", row, "", uiFont, 14f, TextColor, TextAlignmentOptions.MidlineRight);
		valueText.characterSpacing = 8f;
		Place(valueText.rectTransform, new Vector2(1f, 0f), Vector2.one, new Vector2(-ValueWidth, 0f), Vector2.zero);

		BoolControl control = new BoolControl(toggle, track, knob, valueText, get, onText, offText);
		control.Sync();
		control.SnapKnob();
		controls.Add(control);
	}

	void AddRowLabel(RectTransform row, string symbol, string name)
	{
		if (!string.IsNullOrEmpty(symbol))
		{
			// Equation variable : serif symbol + small dimmed name
			TextMeshProUGUI sym = NewText("Symbol", row, symbol, symbolFont, 28f, TextColor, TextAlignmentOptions.MidlineLeft);
			Place(sym.rectTransform, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(44f, 0f));

			TextMeshProUGUI sub = NewText("Name", row, name, uiFont, 13f, DimTextColor, TextAlignmentOptions.MidlineLeft);
			sub.characterSpacing = 4f;
			Place(sub.rectTransform, Vector2.zero, new Vector2(0f, 1f), new Vector2(44f, 0f), new Vector2(LabelWidth - 8f, 0f));
		}
		else
		{
			TextMeshProUGUI label = NewText("Name", row, name, uiFont, 16f, TextColor, TextAlignmentOptions.MidlineLeft);
			Place(label.rectTransform, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(LabelWidth - 8f, 0f));
		}
	}

	RectTransform NewRow(string name, float height)
	{
		RectTransform row = NewRect(name, currentColumn);
		LayoutElement layout = row.gameObject.AddComponent<LayoutElement>();
		layout.minHeight = height;
		layout.preferredHeight = height;
		return row;
	}

	void AddFrame(RectTransform rt)
	{
		Place(NewImage("Frame Top", rt, LineColor).rectTransform, new Vector2(0f, 1f), Vector2.one, new Vector2(0f, -1f), Vector2.zero);
		Place(NewImage("Frame Bottom", rt, LineColor).rectTransform, Vector2.zero, new Vector2(1f, 0f), Vector2.zero, new Vector2(0f, 1f));
		Place(NewImage("Frame Left", rt, LineColor).rectTransform, Vector2.zero, new Vector2(0f, 1f), Vector2.zero, new Vector2(1f, 0f));
		Place(NewImage("Frame Right", rt, LineColor).rectTransform, new Vector2(1f, 0f), Vector2.one, new Vector2(-1f, 0f), Vector2.zero);
	}

	// ---------------------------------------------------------------- Helpers

	static RectTransform NewRect(string name, Transform parent)
	{
		GameObject go = new GameObject(name, typeof(RectTransform));
		go.layer = LayerMask.NameToLayer("UI");
		RectTransform rt = (RectTransform)go.transform;
		rt.SetParent(parent, false);
		return rt;
	}

	static Image NewImage(string name, Transform parent, Color color)
	{
		Image image = NewRect(name, parent).gameObject.AddComponent<Image>();
		image.color = color;
		return image;
	}

	static TextMeshProUGUI NewText(string name, Transform parent, string text, TMP_FontAsset font, float size, Color color, TextAlignmentOptions alignment)
	{
		TextMeshProUGUI tmp = NewRect(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
		if (font != null) tmp.font = font;
		tmp.text = text;
		tmp.fontSize = size;
		tmp.color = color;
		tmp.alignment = alignment;
		tmp.textWrappingMode = TextWrappingModes.NoWrap;
		tmp.overflowMode = TextOverflowModes.Overflow;
		tmp.raycastTarget = false;
		return tmp;
	}

	static void Place(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
	{
		rt.anchorMin = anchorMin;
		rt.anchorMax = anchorMax;
		rt.offsetMin = offsetMin;
		rt.offsetMax = offsetMax;
	}

	static void Stretch(RectTransform rt)
	{
		Place(rt, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
	}

	static TMP_FontAsset FindFont(params string[] names)
	{
		TMP_FontAsset[] loaded = Resources.FindObjectsOfTypeAll<TMP_FontAsset>();
		foreach (string fontName in names)
		{
			foreach (TMP_FontAsset font in loaded)
			{
				if (font.name == fontName) return font;
			}
		}
		return TMP_Settings.defaultFontAsset;
	}

	static Sprite CreateCircleSprite(int size)
	{
		Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
		tex.wrapMode = TextureWrapMode.Clamp;
		tex.filterMode = FilterMode.Bilinear;
		float r = size * 0.5f;
		Color32[] pixels = new Color32[size * size];
		for (int y = 0; y < size; y++)
		{
			for (int x = 0; x < size; x++)
			{
				float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
				byte alpha = (byte)(Mathf.Clamp01(r - d) * 255f);
				pixels[y * size + x] = new Color32(255, 255, 255, alpha);
			}
		}
		tex.SetPixels32(pixels);
		tex.Apply();
		float border = r - 1f;
		return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(border, border, border, border));
	}

	static void EnsureEventSystem()
	{
		if (EventSystem.current != null || FindAnyObjectByType<EventSystem>() != null) return;
		GameObject go = new GameObject("EventSystem");
		go.AddComponent<EventSystem>();
		go.AddComponent<StandaloneInputModule>();
	}

	// ---------------------------------------------------------------- Controls

	private abstract class Control
	{
		public abstract void Sync();
	}

	private class FloatControl : Control
	{
		private readonly Slider slider;
		private readonly PressTracker press;
		private readonly TextMeshProUGUI valueText;
		private readonly Func<float> get;
		private readonly string format;
		private float shownValue = float.NaN;

		public FloatControl(Slider slider, PressTracker press, TextMeshProUGUI valueText, Func<float> get, string format)
		{
			this.slider = slider;
			this.press = press;
			this.valueText = valueText;
			this.get = get;
			this.format = format;
		}

		public override void Sync()
		{
			float v = get();
			// Don't fight the user while they drag
			if (!press.pressed && slider.value != v) slider.SetValueWithoutNotify(v);
			if (v != shownValue)
			{
				shownValue = v;
				valueText.text = v.ToString(format, CultureInfo.InvariantCulture);
			}
		}
	}

	private class BoolControl : Control
	{
		private readonly Toggle toggle;
		private readonly Image track;
		private readonly Image knob;
		private readonly TextMeshProUGUI valueText;
		private readonly Func<bool> get;
		private readonly string onText;
		private readonly string offText;

		public BoolControl(Toggle toggle, Image track, Image knob, TextMeshProUGUI valueText, Func<bool> get, string onText, string offText)
		{
			this.toggle = toggle;
			this.track = track;
			this.knob = knob;
			this.valueText = valueText;
			this.get = get;
			this.onText = onText;
			this.offText = offText;
		}

		private float KnobTargetX(bool on) { return on ? 30f : 10f; }

		public void SnapKnob()
		{
			knob.rectTransform.anchoredPosition = new Vector2(KnobTargetX(get()), 0f);
		}

		public override void Sync()
		{
			bool on = get();
			if (toggle.isOn != on) toggle.SetIsOnWithoutNotify(on);

			track.color = on ? FillColor : TrackColor;
			knob.color = on ? Color.black : new Color(1f, 1f, 1f, 0.8f);
			valueText.text = on ? onText : offText;

			Vector2 pos = knob.rectTransform.anchoredPosition;
			pos.x = Mathf.Lerp(pos.x, KnobTargetX(on), 1f - Mathf.Exp(-25f * Time.unscaledDeltaTime));
			knob.rectTransform.anchoredPosition = pos;
		}
	}
}

// Tracks whether a slider is currently held, so live OSC sync doesn't fight the mouse
public class PressTracker : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public bool pressed;
	public void OnPointerDown(PointerEventData eventData) { pressed = true; }
	public void OnPointerUp(PointerEventData eventData) { pressed = false; }
	void OnDisable() { pressed = false; }
}

