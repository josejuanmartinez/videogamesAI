using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public List<string> mouseSpritesNames;
    public List<Texture2D> mouseSpritesSprites;

    bool defaultCursor;
    string currentCursor;

    public void Awake()
    {
        currentCursor = string.Empty;
    }

    public void Update()
    {
        defaultCursor = currentCursor == string.Empty;
    }

    public void ChangeCursor(string spriteId)
    {
        if (spriteId == null)
        {
            RemoveCursor();
            return;
        }

        int index = mouseSpritesNames.IndexOf(spriteId);
        if(index == -1)
        {
            RemoveCursor();
            return;
        }
        currentCursor = spriteId;
        Cursor.SetCursor(
            mouseSpritesSprites[index], 
            new Vector2(mouseSpritesSprites[index].width / 2, mouseSpritesSprites[index].height / 2), 
            CursorMode.ForceSoftware
        );
        
    }

    public void RemoveCursor()
    {
        if(!defaultCursor)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
            currentCursor = string.Empty;
        }
    }

    public void RemoveCursor(string spriteId)
    {
        if(spriteId == currentCursor)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
            currentCursor= string.Empty;
        }
    }

    public void Clickable()
    {
        ChangeCursor("clickable");
    }

    public void Unclickable()
    {
        ChangeCursor("unclickable");
    }

}
