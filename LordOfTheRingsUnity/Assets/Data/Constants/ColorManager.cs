using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public List<NationsEnum> nations = new();
    public List<Color> nationColors = new();
    
    public List<string> colorStrings= new();
    public List<Color> colors = new();

    public Color GetNationColor(NationsEnum nation)
    {
        int index = nations.IndexOf(nation);
        if(index != -1)
            return nationColors[index];
        return Color.white;
    }

    public Color GetColor(string stringId)
    {
        Color color = Color.white;
        int index = colorStrings.IndexOf(stringId.ToLower());
        if (index != -1)
            color = colors[index];
        else
            Debug.Log(string.Format("{0} color not defined. Consider adding it.", stringId));
        color = new Color(color.r, color.g, color.b, 0.75f);    
        return color;
    }



}
