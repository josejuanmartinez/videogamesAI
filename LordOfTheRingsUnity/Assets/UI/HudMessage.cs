using UnityEngine;

public struct HUDMessage
{
    public string text;
    public Color color;
    public float delay;

    public HUDMessage(string text, float delay, Color color)
    {
        this.text = text;
        this.delay = delay;
        this.color = color;
    }
}