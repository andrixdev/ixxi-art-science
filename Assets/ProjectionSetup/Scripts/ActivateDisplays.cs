using UnityEngine;
using System.Collections;

public class ActivateDisplays : MonoBehaviour
{
	public int displayCount = -1;

	void Start() {
		Debug.Log("displays connected: " + Display.displays.Length);
		// Display.displays[0] is the primary, default display and is always ON, so start at index 1.
		// Check if additional displays are available and activate each.

		int requiredDisplayCount = displayCount;

		if (requiredDisplayCount <= 0 || requiredDisplayCount > Display.displays.Length)
		{
			requiredDisplayCount = Display.displays.Length;
		}

		for (int i = 1; i < requiredDisplayCount; i++) {
			Display.displays[i].Activate();
		}
	}
}