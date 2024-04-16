/**
 * Alex Andrix © 2020-2024
 * This script reads simple position XYZ data
 * It generates 2D textures for interpretation in a Visual Effect Graph
 * Works up to 2M points (limited by max texture width)
 */

using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Windows;

public class XyzDataSmith : MonoBehaviour
{
	public TextAsset textAsset; // test-aa.txt

	// RGBAFloat data works well for values in [-1, 1] so we need some wee data pre-scaling
	private float xyzScale = 0.0001f; // Rescaling for xyz positions ([0, 10000] -> [0, 1.0f])

	[HideInInspector] // Important! (super long data, you don't want it in the Editor)
	public XyzData xyzData;

	[HideInInspector] // Important! (super long data, you don't want it in the Editor)
    public Texture2D texture1; // Will store (x, y, z, ⌀) in RGBA
	[HideInInspector]
    public Texture2D texture2; // Will store (⌀, ⌀, ⌀, ⌀) in RGBA
	[HideInInspector]
	public Texture2D texture3; // Will store (⌀, ⌀, ⌀, ⌀)
	
	private float timeStart;
	
	void OnEnable()
    {
        // Read le file
		ReadBrain();
		
		// Build le textures
		int cubicRoot = 110; // Keep it a little greater than the cubic root of the number of items, max 126
        CreateTextures(cubicRoot);
    }

    void Start()
    {
		
    }

	public void ReadBrain()
	{
		timeStart = Time.realtimeSinceStartup;

		// Split lines
		string[] lines = textAsset.text.Split('\n');

		/**
		 * Line format:
		 * 50 10 340
		 * x y z
		 *
		 * (x, y, z) position
		 */
		xyzData = new XyzData(lines.Length);
		for (int i = 0; i < lines.Length; i++)
		{
			string[] fields = lines[i].Split(' ');
			float x = xyzScale * Convert.ToSingle(fields[0], CultureInfo.InvariantCulture);
			float y = xyzScale * Convert.ToSingle(fields[1], CultureInfo.InvariantCulture);
			float z = xyzScale * Convert.ToSingle(fields[2], CultureInfo.InvariantCulture);
			//float someFlag = someFlagScale * Convert.ToSingle(fields[6], CultureInfo.InvariantCulture);

			XyzBit xyz = new XyzBit();

			xyz.x = x;
			xyz.y = y;
			xyz.z = z;

			xyzData.data[i] = xyz;
		}

		// Success message
		Debug.Log("<color=teal>" + lines.Length + " lines of position data were parsed.</color>");

		// Random data log
		int testIndex = Convert.ToInt32(Math.Floor(lines.Length * 0.75));
		XyzBit testXyz = xyzData.data[testIndex];
		Debug.Log("<color=teal>Logging position bit data for index " + testIndex +  ": x = " + testXyz.x + ", y = " + testXyz.y + ", z = " + testXyz.z + "</color>");
		
		// Log time taken
		Debug.Log("<color=teal>XyzDataSmith forged data in " + (Time.realtimeSinceStartup - timeStart) + " seconds.</color>");
	}

	void CreateTextures(int size)
	{
		timeStart = Time.realtimeSinceStartup;

		Color[] colorArray1 = new Color[size * size * size];
		Color[] colorArray2 = new Color[size * size * size];
		Color[] colorArray3 = new Color[size * size * size];
		
		// RGBAHalf is sufficient as we're using floats to parse data and not doubles, RGBAFloat might be needed if we parse to doubles
		texture1 = new Texture2D(size * size, size, TextureFormat.RGBAHalf, true); 
		texture2 = new Texture2D(size * size, size, TextureFormat.RGBAHalf, true);
		texture3 = new Texture2D(size * size, size, TextureFormat.RGBAHalf, true);
		
		// Single loop to inject data array values in textures
		float r = 1.0f / (size - 1.0f);
        for (int x = 0; x < size; x++)
		{
            for (int y = 0; y < size; y++)
			{
                for (int z = 0; z < size; z++)
				{
					int index = x + (y * size) + (z * size * size);
					Color c1 = new Color(0, 0, 0, 0);
					Color c2 = new Color(0, 0, 0, 0);
					Color c3 = new Color(0, 0, 0, 0);
					
					XyzBit xyz;

					if (index < xyzData.data.Length)
					{
						xyz = xyzData.data[index];
					}
					else
					{
						xyz = new XyzBit();
						xyz.x = 0;
						xyz.y = 0;
						xyz.z = 0;
					}

					// Shape color information for textures
					c1 = new Color(xyz.x, xyz.y, xyz.z, 0);
					c2 = new Color(0, 0, 0, 0);
					c3 = new Color(0, 0, 0, 0);
					
                    colorArray1[index] = c1;
                    colorArray2[index] = c2;
                    colorArray3[index] = c3;
                }
            }
        }
		
		// Inject in textures
		texture1.SetPixels(colorArray1);
		texture2.SetPixels(colorArray2);
		texture3.SetPixels(colorArray3);
		texture1.Apply();
		texture2.Apply();
		texture3.Apply();

		// Log time taken
		Debug.Log("<color=teal>XyzDataSmith generated textures in " + (Time.realtimeSinceStartup - timeStart) + " seconds 💪</color>");
	}
}
