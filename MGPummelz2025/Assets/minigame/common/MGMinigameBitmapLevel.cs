﻿using UnityEngine;
using System.Collections;
using rccg.frontend;

public class MGMinigameBitmapLevel
{
    private Texture2D levelTexture;

    private string game;
    private string levelName;

    public MGMinigameBitmapLevel(string game, string level)
    {
        this.game = game;
        this.levelName = level;
    }

    public void load()
    {
        if(this.levelTexture == null)
        {
            this.levelTexture = GUIResourceLoader.getResourceLoaderInstance().loadMinigameLevel(game, levelName);
        }
    }


    public Color get(int x, int y)
    {
        Color c = levelTexture.GetPixel(x, y);
        return c;
       
    }

   

}
