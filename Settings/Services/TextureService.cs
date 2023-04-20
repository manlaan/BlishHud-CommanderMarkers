﻿using System;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace Manlaan.CommanderMarkers.Settings.Services;

public class TextureService : IDisposable
{
    public TextureService(ContentsManager contentsManager)
    {
        CornerIconTexture = contentsManager.GetTexture(@"raids\textures\raidIconDark.png");
        CornerIconHoverTexture = contentsManager.GetTexture(@"raids\textures\raidIconBright.png");

        _imgArrow = contentsManager.GetTexture(@"arrow.png");
        _imgCircle = contentsManager.GetTexture(@"circle.png");
        _imgHeart = contentsManager.GetTexture(@"heart.png");
        _imgSpiral = contentsManager.GetTexture(@"spiral.png");
        _imgSquare = contentsManager.GetTexture(@"square.png");
        _imgStar = contentsManager.GetTexture(@"star.png");
        _imgTriangle = contentsManager.GetTexture(@"triangle.png");
        _imgX = contentsManager.GetTexture(@"x.png");
        _imgClear = contentsManager.GetTexture(@"clear.png");

    }

    public void Dispose()
    {
        CornerIconTexture.Dispose();
        CornerIconHoverTexture.Dispose();

        _imgArrow.Dispose();
        _imgCircle.Dispose();
        _imgHeart.Dispose();    
        _imgSpiral.Dispose();
        _imgSquare.Dispose();
        _imgStar.Dispose();
        _imgTriangle.Dispose();
        _imgX.Dispose();
        _imgClear.Dispose();
    }

    public Texture2D _imgArrow;
    public Texture2D _imgCircle;
    public Texture2D _imgHeart;
    public Texture2D _imgSpiral;
    public Texture2D _imgSquare;
    public Texture2D _imgStar;
    public Texture2D _imgTriangle;
    public Texture2D _imgX;
    public Texture2D _imgClear;

   
    public Texture2D CornerIconTexture { get; }
    public Texture2D CornerIconHoverTexture { get; }

}