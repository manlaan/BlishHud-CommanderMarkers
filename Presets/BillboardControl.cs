using Blish_HUD;
using Blish_HUD.Controls;
using Manlaan.CommanderMarkers.Presets.Model;
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
    public override void DoUpdate(GameTime gameTime)
    {
        base.DoUpdate(gameTime);
        Size = Parent.Size;
    }

    protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
    {
/*#if DEBUG
        ClipsBounds = false;
        spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, bounds, new Color(96, 96, 96, 192));
        var screen = ScreenMap.Data.ScreenBounds;
        spriteBatch.DrawStringOnCtrl(this, $"{screen}\n({screen.X + screen.Width}),({screen.Y + screen.Height})", _bitmapFont, bounds, Color.Orange, horizontalAlignment: Blish_HUD.Controls.HorizontalAlignment.Left, verticalAlignment: VerticalAlignment.Bottom);
        Point spriteScreenSizez = Graphics.SpriteScreen.Size;
        for(int i = 0; i < (int)(spriteScreenSizez.X / 50); i++)
        {
            spriteBatch.DrawOnCtrl(Graphics.SpriteScreen,ContentService.Textures.Pixel, new Rectangle(i*50, 0, 2, Graphics.SpriteScreen.Height), i%2==0?Color.Red:Color.Pink);

        }
#endif*/
        if (!GameIntegration.Gw2Instance.IsInGame || _mapData.Current == null)
            return;
        if (GameService.Gw2Mumble.PlayerCharacter.IsInCombat) return;
        if (
            (Service.Settings.AutoMarker_OnlyWhenCommander.Value && !GameService.Gw2Mumble.PlayerCharacter.IsCommander) 
            &&  !Service.LtMode.Value
            ) return;
    
        var playerPosition = GameService.Gw2Mumble.PlayerCharacter.Position;
            
        bounds.Location = Location;
 
        foreach (var entity in _entities)
        {
            entity.Draw();
            
        }
        
    }
}