/**
 * Alex Andrix © 2020-2024
 */

using UnityEngine;

[System.Serializable]
public struct XyzData
{
	public XyzData(int size)
	{
		data = new XyzBit[size];
		title = "Xyz data from .tif format given by Marie Monniaux - Processed by Alex Andrix for CNRS/Planétarium de Vaulx-en-Velin";
		version = "1.0";
		date = "2024-03-01";
	}

	public string title;
	public string version;
	public string date;
	public XyzBit[] data;
}
