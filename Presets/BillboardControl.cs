using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Presets.Model;
using Manlaan.CommanderMarkers.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;
using static Blish_HUD.GameService;

namespace Manlaan.CommanderMarkers.Presets;

public class BillboardControl : Control
{
    private readonly List<BillBoardPreview> _entities = new();
    private readonly MapData _mapData;

    //private AsyncTexture2D _interactBackground = AsyncTexture2D.FromAssetId(156112);
    private AsyncTexture2D _interactBackground = AsyncTexture2D.FromAssetId(156775);//https://assets.gw2dat.com/156775.png
    //private AsyncTexture2D _interactBackground = Service.Textures!._interactBg;
    private Texture2D? _invertedTexture;
    private bool _inverted = false;
    public BillboardControl(MapData mapData)
    {
        _mapData = mapData;
    }

    protected override CaptureType CapturesInput() => CaptureType.None;
    public void AddEntity(BillBoardPreview entity)
        => _entities.Add(entity);

    public void RemoveEntity(BillBoardPreview entity)
        => _entities.Remove(entity);

    public void ClearEntities() => _entities.Clear();

    private void InvertTexture(AsyncTexture2D texture)
    {
        if (!texture.HasTexture) return;

        var originalTexture = texture.Texture;
        var width = originalTexture.Width;
        var height = originalTexture.Height;

        // Get the texture data
        var data = new Color[width * height];
        originalTexture.GetData(data);

        // Invert each pixel's color values
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = new Color(
                255 - data[i].R,  // Invert red
                255 - data[i].G,  // Invert green
                255 - data[i].B,  // Invert blue
                data[i].A         // Keep alpha unchanged
            );
        }

        // Create a new texture with the inverted data using the proper graphics device access
        using (var graphicsDeviceContext = GameService.Graphics.LendGraphicsDeviceContext())
        {
            var invertedTexture = new Texture2D(graphicsDeviceContext.GraphicsDevice, width, height);
            invertedTexture.SetData(data);

            // Since we can't directly assign to texture.Texture, we'll need to store the inverted texture separately
            // and use it in the Paint method instead
            _invertedTexture = invertedTexture;
        }
    }

    public override void DoUpdate(GameTime gameTime)
    {
        base.DoUpdate(gameTime);
        Size = Parent.Size;

        if(!_inverted && _interactBackground.HasSwapped)
        {
            _inverted = true;
            InvertTexture(_interactBackground);
        }

    }

    protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
    {
        if (!GameIntegration.Gw2Instance.IsInGame || _mapData.Current == null)
            return;
        if (GameService.Gw2Mumble.PlayerCharacter.IsInCombat && !Service.Settings.AutoMarker_Allow_Combat_Placement.Value) return;
        if (
            Service.Settings.AutoMarker_OnlyWhenCommander.Value && !CommanderPermissionHelper.HasCommanderPermissions()
            ) return;
        if (GameService.Gw2Mumble.UI.IsMapOpen) return;
    
        var playerPosition = GameService.Gw2Mumble.PlayerCharacter.Position;
            
        bounds.Location = Location;
 
        foreach (var entity in _entities)
        {
            entity.Draw();
            if (Service.Settings.AutoMarker_Billboard_Placement.Value && entity.PlayerWithinTriggerDistance())
            {
                Rectangle _promptRectangle = new Rectangle(GameService.Graphics.SpriteScreen.Width / 2 + 150, GameService.Graphics.SpriteScreen.Height/2 + 120, 300, 150);
                Rectangle _textRectangle = new Rectangle(GameService.Graphics.SpriteScreen.Width / 2 + 220, GameService.Graphics.SpriteScreen.Height/2 + 110, 300, 150);
                //InvertTexture(_interactBackground);
                var textureToUse = _interactBackground;
                var interactKey = Service.Settings._settingInteractKeyBinding.Value.GetBindingDisplayText();
                var _bitmapFont = ContentService.Content.DefaultFont18;
                spriteBatch.Draw(textureToUse, _promptRectangle, Color.White);
                spriteBatch.DrawStringOnCtrl(this, $"Press '{interactKey}' to place markers\n{entity.GetMarkerText()}", _bitmapFont, _textRectangle, Color.Black, false, true,2, horizontalAlignment: Blish_HUD.Controls.HorizontalAlignment.Left, verticalAlignment: VerticalAlignment.Middle);
                spriteBatch.DrawStringOnCtrl(this, $"Press '{interactKey}' to place markers\n{entity.GetMarkerText()}", _bitmapFont, _textRectangle, Color.White, horizontalAlignment: Blish_HUD.Controls.HorizontalAlignment.Left, verticalAlignment: VerticalAlignment.Middle);

            }
        }
        
    }



    protected override void DisposeControl()
    {
        _invertedTexture?.Dispose();
        base.DisposeControl();
    }
}