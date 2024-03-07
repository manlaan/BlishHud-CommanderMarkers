using System;
using Blish_HUD.Modules.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace Manlaan.CommanderMarkers.Settings.Services;

public class TextureService : IDisposable
{
    public TextureService(ContentsManager contentsManager)
    {
        SettingWindowBackground = contentsManager.GetTexture(@"window\background.png");


        _imgArrow = contentsManager.GetTexture(@"arrow.png");
        _imgCircle = contentsManager.GetTexture(@"circle.png");
        _imgHeart = contentsManager.GetTexture(@"heart.png");
        _imgSpiral = contentsManager.GetTexture(@"spiral.png");
        _imgSquare = contentsManager.GetTexture(@"square.png");
        _imgStar = contentsManager.GetTexture(@"star.png");
        _imgTriangle = contentsManager.GetTexture(@"triangle.png");
        _imgX = contentsManager.GetTexture(@"x.png");

        _imgArrowFade = contentsManager.GetTexture(@"arrow_fade.png");
        _imgCircleFade = contentsManager.GetTexture(@"circle_fade.png");
        _imgHeartFade = contentsManager.GetTexture(@"heart_fade.png");
        _imgSpiralFade = contentsManager.GetTexture(@"spiral_fade.png");
        _imgSquareFade = contentsManager.GetTexture(@"square_fade.png");
        _imgStarFade = contentsManager.GetTexture(@"star_fade.png");
        _imgTriangleFade = contentsManager.GetTexture(@"triangle_fade.png");
        _imgXFade = contentsManager.GetTexture(@"x_fade.png");

        _imgClear = contentsManager.GetTexture(@"clear.png");
        _imgCheck =contentsManager.GetTexture(@"check.png");
        _blishHeart = contentsManager.GetTexture(@"mapmarker.png");
        _blishHeartSmall = contentsManager.GetTexture(@"mapmarker20.png");

        IconEye = contentsManager.GetTexture(@"eye.png");

        IconCopy = contentsManager.GetTexture(@"iconCopy.png");
        IconDelete = contentsManager.GetTexture(@"iconDelete.png");
        IconDeleteLarge = contentsManager.GetTexture(@"iconDelete48.png");
        IconEdit = contentsManager.GetTexture(@"iconEdit.png");
        IconExport = contentsManager.GetTexture(@"iconExport.png");
        IconGoBack = contentsManager.GetTexture(@"iconGoBack.png");
        IconImport = contentsManager.GetTexture(@"iconImport.png");
        IconSave = contentsManager.GetTexture(@"iconSave.png");

        IconCorner = contentsManager.GetTexture(@"cornerIcon.png");

    }

    public void Dispose()
    {
        SettingWindowBackground?.Dispose(); 
        _imgArrow?.Dispose();
        _imgCircle?.Dispose();
        _imgHeart?.Dispose();    
        _imgSpiral?.Dispose();
        _imgSquare?.Dispose();
        _imgStar?.Dispose();
        _imgTriangle?.Dispose();
        _imgX?.Dispose();
        _imgArrowFade?.Dispose();
        _imgCircleFade?.Dispose();
        _imgHeartFade?.Dispose();
        _imgSpiralFade?.Dispose();
        _imgSquareFade?.Dispose();
        _imgStarFade?.Dispose();
        _imgTriangleFade?.Dispose();
        _imgXFade?.Dispose();
        _imgClear?.Dispose();
        _imgCheck?.Dispose();
        _blishHeart?.Dispose();
        _blishHeartSmall?.Dispose();

        IconEye?.Dispose();

        IconCopy?.Dispose();
        IconDelete?.Dispose();
        IconDeleteLarge?.Dispose();
        IconEdit?.Dispose();
        IconExport?.Dispose();
        IconGoBack?.Dispose();
        IconImport?.Dispose();
        IconSave?.Dispose();

        IconCorner?.Dispose();
}

    public Texture2D SettingWindowBackground;
    public Texture2D _imgArrow;
    public Texture2D _imgCircle;
    public Texture2D _imgHeart;
    public Texture2D _imgSpiral;
    public Texture2D _imgSquare;
    public Texture2D _imgStar;
    public Texture2D _imgTriangle;
    public Texture2D _imgX;
    public Texture2D _imgArrowFade;
    public Texture2D _imgCircleFade;
    public Texture2D _imgHeartFade;
    public Texture2D _imgSpiralFade;
    public Texture2D _imgSquareFade;
    public Texture2D _imgStarFade;
    public Texture2D _imgTriangleFade;
    public Texture2D _imgXFade;
    public Texture2D _imgClear;
    public Texture2D _imgCheck;
    public Texture2D _blishHeart;
    public Texture2D _blishHeartSmall;

    public Texture2D IconEye;
    public Texture2D IconCopy;
    public Texture2D IconDelete;
    public Texture2D IconDeleteLarge;
    public Texture2D IconEdit;
    public Texture2D IconExport;
    public Texture2D IconGoBack;
    public Texture2D IconImport;
    public Texture2D IconSave;

    public Texture2D IconCorner;


}