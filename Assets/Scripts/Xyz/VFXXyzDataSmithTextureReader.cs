/**
 * Alex Andrix © 2023-2024
 * Add this script to the VFX GameObject
 * It loads the input textures into the VFX
 */

using System;
using UnityEngine;
using UnityEngine.VFX;

public class VFXXyzDataSmithTextureReader : MonoBehaviour
{
	public XyzDataSmith dataSmith;

	void Start()
	{

	}

	void OnEnable()
    {
		// Get this GameObject's VFX
		VisualEffect vfx = (VisualEffect) GetComponent<VisualEffect>();

		// Set its input data textures with what the data smith has prepped for us
		vfx.SetTexture("Texture1", dataSmith.texture1);
		vfx.SetTexture("Texture2", dataSmith.texture2);
		vfx.SetTexture("Texture3", dataSmith.texture3);
    }

}
