/**
 * Custom class to store Equation Variable information with extra details
 * ANDRIX © 2024
 */
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayVariable : IComparable<PlayVariable>
{
	public string name;
    public float value;
    public float time;
    private float duration; // Opacity fadeout duration

    public PlayVariable(string name, float value)
    {
        this.name = name;
        this.value = value;
        this.time = Time.time;
        this.duration = 4.0f;
    }

    public void update(float newValue)
    {
        this.value = newValue;
        this.time = Time.time;
    }

    public string getText()
    {
        return this.name + "   =   " + Mathf.Round(100 * this.value) / 100;
    }

    public float getOpacity()
    {
        return Mathf.Clamp(1.0f - (Time.time - this.time) / this.duration, 0, 1.0f);
    }

    public int CompareTo(PlayVariable otherPlayVar)
    {
        return -this.time.CompareTo(otherPlayVar.time);
    }
}
